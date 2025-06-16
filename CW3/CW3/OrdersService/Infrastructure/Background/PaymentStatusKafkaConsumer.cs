using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrdersService.Infrastructure.Repositories;
using OrdersService.Domain.Entities;
using System.Text.Json;

namespace OrdersService.Infrastructure.Background
{
    public class PaymentStatusKafkaConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentStatusKafkaConsumer> _logger;
        private readonly string _topic = "payments-events";
        private readonly string _groupId = "orders-payment-status-consumer";
        private readonly string _bootstrapServers;

        public PaymentStatusKafkaConsumer(IServiceProvider serviceProvider, ILogger<PaymentStatusKafkaConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "kafka:9092";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PaymentStatusKafkaConsumer started");
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
                    var cr = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (cr != null)
                    {
                        var evt = JsonSerializer.Deserialize<PaymentStatusEvent>(cr.Message.Value);
                        if (evt != null && (evt.Status == "finished" || evt.Status == "cancelled"))
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var orderRepo = scope.ServiceProvider.GetRequiredService<OrderRepository>();
                            var order = await orderRepo.GetByIdAsync(evt.OrderId);
                            if (order != null)
                            {
                                order.Status = evt.Status == "finished" ? OrderStatus.Finished : OrderStatus.Cancelled;
                                await orderRepo.UpdateAsync(order);
                                _logger.LogInformation($"Order {order.Id} status updated to {order.Status}");
                            }
                            else
                            {
                                _logger.LogWarning($"Order {evt.OrderId} not found for payment status event");
                            }
                        }
                        consumer.Commit(cr);
                        _logger.LogInformation($"Consumed payment status event: {cr.Message.Value}");
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
                    _logger.LogError(ex, "Ошибка обработки события оплаты из Kafka");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private class PaymentStatusEvent
        {
            public Guid OrderId { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}
