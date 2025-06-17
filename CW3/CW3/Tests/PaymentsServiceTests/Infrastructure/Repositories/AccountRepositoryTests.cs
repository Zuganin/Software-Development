using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Repositories;
using PaymentsService.Domain.Model.Entities;
using PaymentsService.Infrastructure.DbContext;

namespace PaymentsServiceTests.Infrastructure.Repositories
{
    public class AccountRepositoryTests
    {
        [Fact]
        public async Task AddAndGetByUserIdAsync_WorksCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var db = new PaymentsDbContext(options);
            var repo = new AccountRepository(db);
            var userId = Guid.NewGuid();
            var acc = Account.Create(userId);
            acc.Deposit(100);

            // Act
            await repo.AddAsync(acc, default);
            var found = await repo.GetByUserIdAsync(acc.UserId, default);

            // Assert
            Assert.NotNull(found);
            Assert.Equal(acc.UserId, found.UserId);
            Assert.Equal(100, found.Balance);
        }
    }
}
