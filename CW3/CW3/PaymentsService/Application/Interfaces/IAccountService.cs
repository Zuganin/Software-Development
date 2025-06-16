namespace PaymentsService.Application.Interfaces;

public interface IAccountService
{
    Task CreateAccountAsync(Guid userId, CancellationToken cancellationToken);
    Task<decimal> DepositAsync(Guid userId, decimal amount, CancellationToken cancellationToken);
    Task<decimal> WithdrawAsync(Guid userId, decimal amount, CancellationToken cancellationToken);
    Task<decimal> GetBalanceAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<PaymentsService.Domain.Model.Entities.Transaction>> GetTransactionsAsync(Guid userId, CancellationToken cancellationToken);
}