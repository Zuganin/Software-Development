using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Interfaces;
using PaymentsService.Domain.Model.Entities;
using PaymentsService.Domain.Model.Interfaces;
using PaymentsService.Infrastructure.DbContext;
using System.Text.Json;
using PaymentsService.Domain.Events;


namespace PaymentsService.Application.Services;


public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly PaymentsDbContext _dbContext;

    public AccountService(IAccountRepository repository, IOutboxRepository outboxRepository, PaymentsDbContext dbContext)
    {
        _repository = repository;
        _outboxRepository = outboxRepository;
        _dbContext = dbContext;
    }

    public async Task CreateAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
        {
            using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            var newAccount = Account.Create(userId);
            await _repository.AddAsync(newAccount, cancellationToken);
            var evt = new OutboxEvent
            {
                EventType = "AccountCreated",
                Payload = JsonSerializer.Serialize(new { userId }),
                CorrelationId = userId.ToString()
            };
            await _outboxRepository.AddAsync(evt, cancellationToken);
            await tx.CommitAsync(cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DepositAsync(Guid userId, decimal amount, CancellationToken cancellationToken)
    {
        using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        var account = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found for user");
        account.Deposit(amount);
        await _repository.UpdateAsync(account, cancellationToken);
        var evt = new OutboxEvent
        {
            EventType = "AccountCredited",
            Payload = JsonSerializer.Serialize(new { userId, amount }),
            CorrelationId = userId.ToString()
        };
        await _outboxRepository.AddAsync(evt, cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }

    public async Task WithdrawAsync(Guid userId, decimal amount, CancellationToken cancellationToken)
    {
        using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        var account = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found for user");
        account.Withdraw(amount);
        await _repository.UpdateAsync(account, cancellationToken);
        var evt = new OutboxEvent
        {
            EventType = "AccountDebited",
            Payload = JsonSerializer.Serialize(new { userId, amount }),
            CorrelationId = userId.ToString()
        };
        await _outboxRepository.AddAsync(evt, cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }

    public async Task<decimal> GetBalanceAsync(Guid userId, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return account?.GetBalance() ?? 0m;
    }

}