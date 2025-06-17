
using Confluent.Kafka;
using PaymentsService.Domain.Events;
using PaymentsService.Domain.Model.Interfaces;


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
                    var cr = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (cr != null)
                    {
                        _logger.LogInformation($"InboxKafkaConsumer: получено событие из Kafka: Key={cr.Message.Key}, Value={cr.Message.Value}");
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
                        await inboxRepository.AddAsync(inboxEvent, stoppingToken);
                        _logger.LogInformation($"InboxKafkaConsumer: событие сохранено в InboxEvents с Id={inboxEvent.Id}, статус Pending");
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
