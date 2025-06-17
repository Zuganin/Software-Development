using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrdersService.Domain.Entities;
using OrdersService.Infrastructure.DatabaseContext;
using OrdersService.Infrastructure.Repositories;
using Xunit;

namespace OrdersServiceTests.Infrastructure.Repositories
{
    public class OrderRepositoryTests
    {
        private OrdersDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new OrdersDbContext(options);
        }

        [Fact]
        public async Task AddAndGetByIdAsync_WorksCorrectly()
        {
            var db = CreateDbContext();
            var repo = new OrderRepository(db);
            var order = new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 100, Description = "Test" };
            await repo.AddAsync(order);
            var loaded = await repo.GetByIdAsync(order.Id);
            Assert.NotNull(loaded);
            Assert.Equal(100, loaded!.Amount);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllOrders()
        {
            var db = CreateDbContext();
            var repo = new OrderRepository(db);
            await repo.AddAsync(new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 10, Description = "A" });
            await repo.AddAsync(new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 20, Description = "B" });
            var all = await repo.GetAllAsync();
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task UpdateAsync_ChangesOrderStatus()
        {
            var db = CreateDbContext();
            var repo = new OrderRepository(db);
            var order = new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 50, Description = "C" };
            await repo.AddAsync(order);
            order.Status = OrderStatus.Finished;
            await repo.UpdateAsync(order);
            var loaded = await repo.GetByIdAsync(order.Id);
            Assert.Equal(OrderStatus.Finished, loaded!.Status);
        }
    }
}
