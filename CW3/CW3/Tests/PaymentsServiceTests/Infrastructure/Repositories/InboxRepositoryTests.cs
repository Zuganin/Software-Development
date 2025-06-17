using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Repositories;
using PaymentsService.Domain.Events;
using PaymentsService.Infrastructure.DbContext;

namespace PaymentsServiceTests.Infrastructure.Repositories
{
    public class InboxRepositoryTests
    {
        [Fact]
        public async Task AddAndGetByIdAsync_WorksCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var db = new PaymentsDbContext(options);
            var repo = new InboxRepository(db);
            var evt = new InboxEvent { Id = Guid.NewGuid(), EventType = "Test", Payload = "{}", ReceivedAt = DateTime.UtcNow };

            // Act
            await repo.AddAsync(evt, default);
            var found = await repo.GetByIdAsync(evt.Id, default);

            // Assert
            Assert.NotNull(found);
            Assert.Equal(evt.Id, found.Id);
        }
    }
}
