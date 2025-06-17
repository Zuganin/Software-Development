using PaymentsService.Domain.Model.Interfaces;


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
                    var accountService = scope.ServiceProvider.GetRequiredService<PaymentsService.Application.Services.AccountService>();
                    var events = await inboxRepository.GetPendingAsync(stoppingToken);

                    foreach (var inboxEvent in events)
                    {
                        try
                        {
                            var orderEvent = System.Text.Json.JsonSerializer.Deserialize<OrderCreatedEvent>(inboxEvent.Payload);
                            string idempotencyKey = orderEvent?.Id.ToString() ?? inboxEvent.Id.ToString();
                            if (orderEvent != null)
                            {
                                _logger.LogInformation($"InboxEventProcessor: обработка события заказа: OrderId={orderEvent.Id}, UserId={orderEvent.UserId}, Amount={orderEvent.Amount}");
                                // Получаем аккаунт через публичный метод
                                var account = await accountService.GetAccountByUserIdAsync(orderEvent.UserId, stoppingToken);
                                if (account == null)
                                    throw new InvalidOperationException("Account not found for user");
                                // Транзакция типа 'order' с описанием
                                var orderTransaction = new PaymentsService.Domain.Model.Entities.Transaction
                                {
                                    Id = Guid.NewGuid(),
                                    AccountId = account.Id,
                                    Amount = 0, // не влияет на баланс
                                    Type = "order",
                                    Description = orderEvent.Description,
                                    OccurredOn = DateTime.UtcNow,
                                    IdempotencyKey = $"order-{orderEvent.Id}"
                                };
                                await accountService.AddTransactionAsync(orderTransaction, stoppingToken);
                                // Транзакция списания
                                await accountService.WithdrawAsync(orderEvent.UserId, orderEvent.Amount, orderEvent.Id.ToString(), stoppingToken);
                                // Публикация события оплаты в outbox
                                var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                                var paymentStatusEvent = new {
                                    OrderId = orderEvent.Id,
                                    Status = "finished"
                                };
                                var outboxEvent = new PaymentsService.Domain.Events.OutboxEvent
                                {
                                    EventType = "PaymentFinished",
                                    Payload = System.Text.Json.JsonSerializer.Serialize(paymentStatusEvent),
                                    CorrelationId = orderEvent.Id.ToString()
                                };
                                await outboxRepo.AddAsync(outboxEvent, stoppingToken);
                                inboxEvent.Status = "Processed";
                                inboxEvent.ProcessedAt = DateTime.UtcNow;
                                _logger.LogInformation($"InboxEventProcessor: транзакции 'order' и 'withdraw' созданы, событие оплаты отправлено в outbox для заказа {orderEvent.Id}");
                            }
                            else
                            {
                                inboxEvent.Status = "Error";
                                inboxEvent.Error = "Invalid event payload";
                                _logger.LogWarning($"InboxEventProcessor: ошибка разбора события заказа: payload некорректен");
                            }
                            await inboxRepository.UpdateAsync(inboxEvent, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            inboxEvent.Error = ex.Message;
                            // Счётчик попыток (можно добавить поле Attempts в InboxEvent)
                            int attempts = 1;
                            if (inboxEvent.Error != null && inboxEvent.Error.Contains("Attempts:") && int.TryParse(inboxEvent.Error.Split(':')[1], out int parsed))
                                attempts = parsed + 1;
                            if (attempts >= 5)
                            {
                                inboxEvent.Status = "Error";
                                inboxEvent.Error = $"{ex.Message} (Attempts:{attempts})";
                                _logger.LogError(ex, $"InboxEventProcessor: ошибка бизнес-логики, событие {inboxEvent.Id} переведено в Error после 5 попыток");
                            }
                            else
                            {
                                inboxEvent.Status = "Pending";
                                inboxEvent.Error = $"{ex.Message} (Attempts:{attempts})";
                                _logger.LogWarning(ex, $"InboxEventProcessor: ошибка бизнес-логики, событие {inboxEvent.Id} будет ретраиться, попытка {attempts}");
                            }
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
