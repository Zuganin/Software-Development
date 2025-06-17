using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrdersService.Domain.Events;
using OrdersService.Infrastructure.DatabaseContext;
using OrdersService.Infrastructure.Repositories;
using Xunit;

namespace OrdersServiceTests.Infrastructure.Repositories
{
    public class OutboxRepositoryTests
    {
        private OrdersDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new OrdersDbContext(options);
        }

        [Fact]
        public async Task AddAsync_AddsEventToDb()
        {
            var db = CreateDbContext();
            var repo = new OutboxRepository(db);
            var evt = new OutboxEvent { Id = Guid.NewGuid(), EventType = "Test", Payload = "{}", OccurredOn = DateTime.UtcNow };
            await repo.AddAsync(evt);
            Assert.Single(db.OutboxEvents);
            Assert.Equal(evt.Id, db.OutboxEvents.First().Id);
        }

        [Fact]
        public async Task GetUnprocessedAsync_ReturnsOnlyUnprocessed()
        {
            var db = CreateDbContext();
            db.OutboxEvents.AddRange(new List<OutboxEvent>
            {
                new OutboxEvent { Id = Guid.NewGuid(), Processed = false, OccurredOn = DateTime.UtcNow.AddMinutes(-2) },
                new OutboxEvent { Id = Guid.NewGuid(), Processed = true, OccurredOn = DateTime.UtcNow.AddMinutes(-1) },
                new OutboxEvent { Id = Guid.NewGuid(), Processed = false, OccurredOn = DateTime.UtcNow }
            });
            db.SaveChanges();
            var repo = new OutboxRepository(db);
            var result = await repo.GetUnprocessedAsync();
            Assert.Equal(2, result.Count);
            Assert.All(result, e => Assert.False(e.Processed));
            Assert.True(result[0].OccurredOn <= result[1].OccurredOn);
        }

        [Fact]
        public async Task MarkProcessedAsync_UpdatesEvent()
        {
            var db = CreateDbContext();
            var evt = new OutboxEvent { Id = Guid.NewGuid(), Processed = false, OccurredOn = DateTime.UtcNow };
            db.OutboxEvents.Add(evt);
            db.SaveChanges();
            var repo = new OutboxRepository(db);
            await repo.MarkProcessedAsync(evt);
            var updated = db.OutboxEvents.First(e => e.Id == evt.Id);
            Assert.True(updated.Processed);
            Assert.NotNull(updated.ProcessedOn);
        }
    }
}
