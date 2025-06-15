using Microsoft.EntityFrameworkCore;
using PaymentsService.Domain.Model.Entities;
using PaymentsService.Domain.Model.Interfaces;
using PaymentsService.Infrastructure.DbContext;


namespace PaymentsService.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly PaymentsDbContext _dbContext;

    public AccountRepository(PaymentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.UserId == userId, ct);
    }


    public async Task AddAsync(Account account, CancellationToken cancellationToken)
    {
        _dbContext.Accounts.Add(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken)
    {
        _dbContext.Accounts.Update(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}