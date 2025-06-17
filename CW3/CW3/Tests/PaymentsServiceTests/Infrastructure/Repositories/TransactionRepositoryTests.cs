using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Repositories;
using PaymentsService.Domain.Model.Entities;
using PaymentsService.Infrastructure.DbContext;

namespace PaymentsServiceTests.Infrastructure.Repositories
{
    public class TransactionRepositoryTests
    {
        [Fact]
        public async Task AddAndGetByAccountIdAsync_WorksCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var db = new PaymentsDbContext(options);
            var repo = new TransactionRepository(db);
            var accId = Guid.NewGuid();
            var tx = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = accId,
                Amount = 100,
                Type = "deposit",
                OccurredOn = DateTime.UtcNow
            };

            // Act
            await repo.AddAsync(tx, default);
            var found = await repo.GetByAccountIdAsync(accId, default);

            // Assert
            Assert.Single(found);
            Assert.Equal(100, found[0].Amount);
        }
    }
}
