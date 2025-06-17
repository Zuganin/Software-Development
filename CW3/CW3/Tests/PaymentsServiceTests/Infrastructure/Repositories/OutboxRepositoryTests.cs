using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Repositories;
using PaymentsService.Domain.Events;
using PaymentsService.Infrastructure.DbContext;

namespace PaymentsServiceTests.Infrastructure.Repositories
{
    public class OutboxRepositoryTests
    {
        [Fact]
        public async Task AddAndMarkAsProcessed_WorksCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var db = new PaymentsDbContext(options);
            var repo = new OutboxRepository(db);
            var evt = new OutboxEvent { Id = Guid.NewGuid(), EventType = "Test", Payload = "{}", OccurredOn = DateTime.UtcNow };

            // Act
            await repo.AddAsync(evt, default);
            var unprocessed = db.OutboxEvents.Where(e => !e.Processed).ToList();
            await repo.MarkAsProcessedAsync(evt.Id, default);
            var processed = db.OutboxEvents.Where(e => !e.Processed).ToList();

            // Assert
            Assert.Single(unprocessed);
            Assert.Empty(processed);
        }
    }
}
