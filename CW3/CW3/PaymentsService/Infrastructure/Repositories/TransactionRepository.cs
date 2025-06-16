using PaymentsService.Domain.Model.Entities;
using PaymentsService.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace PaymentsService.Infrastructure.Repositories
{
    public class TransactionRepository
    {
        private readonly PaymentsDbContext _db;
        public TransactionRepository(PaymentsDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Transaction transaction, CancellationToken ct)
        {
            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken ct)
        {
            return await _db.Transactions.Where(t => t.AccountId == accountId).ToListAsync(ct);
        }

        public async Task<decimal> GetBalanceByAccountIdAsync(Guid accountId, CancellationToken ct)
        {
            return await _db.Transactions.Where(t => t.AccountId == accountId).SumAsync(t => t.Amount, ct);
        }

        public async Task<List<Transaction>> GetByUserIdAsync(Guid userId, PaymentsDbContext db, CancellationToken ct)
        {
            var account = await db.Accounts.FirstOrDefaultAsync(a => a.UserId == userId, ct);
            if (account == null) return new List<Transaction>();
            return await _db.Transactions.Where(t => t.AccountId == account.Id).OrderByDescending(t => t.OccurredOn).ToListAsync(ct);
        }

        public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct)
        {
            return await _db.Transactions.FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey, ct);
        }
    }
}
