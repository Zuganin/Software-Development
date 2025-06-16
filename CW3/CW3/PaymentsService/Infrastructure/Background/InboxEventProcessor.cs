using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentsService.Domain.Model.Interfaces;
using PaymentsService.Domain.Events;

namespace PaymentsService.Infrastructure.Background
{
    public class InboxEventProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InboxEventProcessor> _logger;
        private const int DelayMs = 5000;
        private const int BatchSize = 20;

        public InboxEventProcessor(IServiceProvider serviceProvider, ILogger<InboxEventProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var inboxRepository = scope.ServiceProvider.GetRequiredService<IInboxRepository>();
                    var events = await inboxRepository.GetPendingAsync(stoppingToken);

                    foreach (var inboxEvent in events)
                    {
                        try
                        {
                            // TODO: обработка события
                            inboxEvent.Status = "Processed";
                            inboxEvent.ProcessedAt = DateTime.UtcNow;
                            await inboxRepository.UpdateAsync(inboxEvent, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Ошибка обработки InboxEvent {inboxEvent.Id}");
                            inboxEvent.Status = "Error";
                            inboxEvent.Error = ex.Message;
                            await inboxRepository.UpdateAsync(inboxEvent, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка фоновой обработки InboxEvent");
                }

                await Task.Delay(DelayMs, stoppingToken);
            }
        }
    }
}
