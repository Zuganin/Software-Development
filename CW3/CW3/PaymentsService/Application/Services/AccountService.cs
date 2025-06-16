using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Interfaces;
using PaymentsService.Domain.Model.Entities;
using PaymentsService.Domain.Model.Interfaces;
using PaymentsService.Infrastructure.DbContext;
using System.Text.Json;
using PaymentsService.Domain.Events;
using PaymentsService.Infrastructure.Repositories;


namespace PaymentsService.Application.Services;


public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly PaymentsDbContext _dbContext;
    private readonly TransactionRepository _transactionRepository;

    public AccountService(IAccountRepository repository, IOutboxRepository outboxRepository, PaymentsDbContext dbContext, TransactionRepository transactionRepository)
    {
        _repository = repository;
        _outboxRepository = outboxRepository;
        _dbContext = dbContext;
        _transactionRepository = transactionRepository;
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

    public async Task<decimal> DepositAsync(Guid userId, decimal amount, CancellationToken cancellationToken)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Amount to deposit must be greater than zero");
        using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        var account = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found for user");
        account.Deposit(amount);
        await _repository.UpdateAsync(account, cancellationToken);
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = amount,
            Type = "deposit",
            OccurredOn = DateTime.UtcNow
        };
        await _transactionRepository.AddAsync(transaction, cancellationToken);
        var evt = new OutboxEvent
        {
            EventType = "AccountCredited",
            Payload = System.Text.Json.JsonSerializer.Serialize(new { userId, amount }),
            CorrelationId = userId.ToString()
        };
        await _outboxRepository.AddAsync(evt, cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return await _transactionRepository.GetBalanceByAccountIdAsync(account.Id, cancellationToken);
    }

    public async Task<decimal> WithdrawAsync(Guid userId, decimal amount, CancellationToken cancellationToken)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Amount to withdraw must be greater than zero");
        using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        var account = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found for user");
        var balance = await _transactionRepository.GetBalanceByAccountIdAsync(account.Id, cancellationToken);
        if (balance == 0)
            throw new InvalidOperationException("Account balance is zero, cannot withdraw");
        if (balance < amount)
            throw new InvalidOperationException("Insufficient funds");
        account.Withdraw(amount);
        await _repository.UpdateAsync(account, cancellationToken);
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = -amount,
            Type = "withdraw",
            OccurredOn = DateTime.UtcNow
        };
        await _transactionRepository.AddAsync(transaction, cancellationToken);
        var evt = new OutboxEvent
        {
            EventType = "AccountDebited",
            Payload = System.Text.Json.JsonSerializer.Serialize(new { userId, amount }),
            CorrelationId = userId.ToString()
        };
        await _outboxRepository.AddAsync(evt, cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return await _transactionRepository.GetBalanceByAccountIdAsync(account.Id, cancellationToken);
    }

    public async Task<decimal> WithdrawAsync(Guid userId, decimal amount, string idempotencyKey, CancellationToken cancellationToken)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Amount to withdraw must be greater than zero");
        using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        var account = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found for user");
        var existing = await _transactionRepository.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken);
        if (existing != null)
            return await _transactionRepository.GetBalanceByAccountIdAsync(account.Id, cancellationToken); // уже обработано
        var balance = await _transactionRepository.GetBalanceByAccountIdAsync(account.Id, cancellationToken);
        if (balance == 0)
            throw new InvalidOperationException("Account balance is zero, cannot withdraw");
        if (balance < amount)
            throw new InvalidOperationException("Insufficient funds");
        account.Withdraw(amount);
        await _repository.UpdateAsync(account, cancellationToken);
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = -amount,
            Type = "withdraw",
            OccurredOn = DateTime.UtcNow,
            IdempotencyKey = idempotencyKey
        };
        await _transactionRepository.AddAsync(transaction, cancellationToken);
        var evt = new OutboxEvent
        {
            EventType = "AccountDebited",
            Payload = System.Text.Json.JsonSerializer.Serialize(new { userId, amount }),
            CorrelationId = userId.ToString()
        };
        await _outboxRepository.AddAsync(evt, cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return await _transactionRepository.GetBalanceByAccountIdAsync(account.Id, cancellationToken);
    }

    public async Task<decimal> GetBalanceAsync(Guid userId, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (account == null) return 0m;
        return await _transactionRepository.GetBalanceByAccountIdAsync(account.Id, cancellationToken);
    }

    public async Task<List<Transaction>> GetTransactionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _transactionRepository.GetByUserIdAsync(userId, _dbContext, cancellationToken);
    }

}