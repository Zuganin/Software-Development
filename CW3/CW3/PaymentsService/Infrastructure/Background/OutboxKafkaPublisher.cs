using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PaymentsService.Domain.Model.Entities;
using PaymentsService.Infrastructure.DbContext;

namespace PaymentsService.Infrastructure.Background
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
            _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            _topic = configuration["Kafka:Topic"] ?? "payments-events";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };
            using var producer = new ProducerBuilder<string, string>(config).Build();

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                var events = await db.OutboxEvents
                    .Where(e => !e.Processed)
                    .OrderBy(e => e.OccurredOn)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var evt in events)
                {
                    try
                    {
                        var key = evt.CorrelationId ?? evt.Id.ToString();
                        await producer.ProduceAsync(_topic, new Message<string, string> { Key = key, Value = evt.Payload }, stoppingToken);
                        evt.Processed = true;
                        evt.ProcessedOn = DateTime.UtcNow;
                        db.OutboxEvents.Update(evt);
                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"Published event {evt.Id} to Kafka");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to publish event {evt.Id} to Kafka");
                    }
                }
                await Task.Delay(2000, stoppingToken); // Пауза между итерациями
            }
        }
    }
}
