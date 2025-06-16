using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentsService.Domain.Events;
using PaymentsService.Domain.Model.Interfaces;
using System.Text.Json;

namespace PaymentsService.Infrastructure.Background
{
    public class InboxKafkaConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InboxKafkaConsumer> _logger;
        private readonly string _topic = "orders-events"; // Название топика для событий заказов
        private readonly string _groupId = "payments-inbox-consumer";
        private readonly string _bootstrapServers;

        public InboxKafkaConsumer(IServiceProvider serviceProvider, ILogger<InboxKafkaConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "kafka:9092";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            try
            {
                consumer.Subscribe(_topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Kafka subscribe error (topic: {_topic})");
                await Task.Delay(5000, stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(TimeSpan.FromSeconds(1)); // не блокируем поток
                    if (cr != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var inboxRepository = scope.ServiceProvider.GetRequiredService<IInboxRepository>();
                        var inboxEvent = new InboxEvent
                        {
                            Id = Guid.NewGuid(),
                            EventType = "OrderCreated",
                            Payload = cr.Message.Value,
                            Status = "Pending",
                            ReceivedAt = DateTime.UtcNow
                        };
                        var accountService = scope.ServiceProvider.GetRequiredService<PaymentsService.Application.Services.AccountService>();
                        var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(cr.Message.Value);
                        string idempotencyKey = orderEvent?.Id.ToString() ?? inboxEvent.Id.ToString();
                        try
                        {
                            if (orderEvent != null)
                            {
                                await accountService.WithdrawAsync(orderEvent.UserId, orderEvent.Amount, idempotencyKey, stoppingToken);
                                inboxEvent.Status = "Processed";
                                inboxEvent.ProcessedAt = DateTime.UtcNow;
                            }
                            else
                            {
                                inboxEvent.Status = "Error";
                                inboxEvent.Error = "Invalid event payload";
                            }
                        }
                        catch (Exception ex)
                        {
                            inboxEvent.Status = "Error";
                            inboxEvent.Error = ex.Message;
                        }
                        await inboxRepository.AddAsync(inboxEvent, stoppingToken);
                        consumer.Commit(cr);
                    }
                }
                catch (ConsumeException ex) when (ex.Error.Reason.Contains("Unknown topic") || ex.Error.Reason.Contains("Unknown partition"))
                {
                    _logger.LogWarning($"Kafka topic not available: {ex.Error.Reason}. Retrying in 5s...");
                    await Task.Delay(5000, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error");
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка обработки сообщения из Kafka");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }

    public class OrderCreatedEvent
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
