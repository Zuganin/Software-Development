using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using OrdersService.Infrastructure.Repositories;
using OrdersService.Domain.Events;

namespace OrdersService.Infrastructure.Background
{
    public class OutboxKafkaPublisher : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxKafkaPublisher> _logger;
        private readonly string _bootstrapServers;
        private readonly string _topic;

        public OutboxKafkaPublisher(IServiceProvider serviceProvider, ILogger<OutboxKafkaPublisher> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
            _topic = configuration["Kafka:Topic"] ?? "orders-events";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };
            using var producer = new ProducerBuilder<string, string>(config).Build();

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxRepo = scope.ServiceProvider.GetRequiredService<OutboxRepository>();
                var events = await outboxRepo.GetUnprocessedAsync();
                foreach (var evt in events)
                {
                    try
                    {
                        await producer.ProduceAsync(_topic, new Message<string, string> { Key = evt.Id.ToString(), Value = evt.Payload }, stoppingToken);
                        await outboxRepo.MarkProcessedAsync(evt);
                        _logger.LogInformation($"Published event {evt.Id} to Kafka");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to publish event {evt.Id} to Kafka");
                    }
                }
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
