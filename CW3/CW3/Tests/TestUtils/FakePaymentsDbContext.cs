using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PaymentsService.Infrastructure.DbContext;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Tests.TestUtils
{
    public class FakePaymentsDbContext : PaymentsDbContext
    {
        public FakePaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }
        public override DatabaseFacade Database => new FakeDatabaseFacade(this);
    }

    public class FakeDatabaseFacade : DatabaseFacade
    {
        public FakeDatabaseFacade(DbContext context) : base(context) { }
        public override Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IDbContextTransaction>(new FakeDbContextTransaction());
        }
    }

    public class FakeDbContextTransaction : IDbContextTransaction
    {
        public Guid TransactionId => Guid.NewGuid();
        public void Dispose() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public void Commit() { }
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Rollback() { }
        public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
