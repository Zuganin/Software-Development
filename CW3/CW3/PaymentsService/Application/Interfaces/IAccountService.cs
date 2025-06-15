namespace PaymentsService.Application.Interfaces;

public interface IAccountService
{
    Task CreateAccountAsync(Guid userId, CancellationToken cancellationToken);
    Task DepositAsync(Guid userId, decimal amount, CancellationToken cancellationToken);
    Task WithdrawAsync(Guid userId, decimal amount, CancellationToken cancellationToken);
    Task<decimal> GetBalanceAsync(Guid userId, CancellationToken cancellationToken);
}