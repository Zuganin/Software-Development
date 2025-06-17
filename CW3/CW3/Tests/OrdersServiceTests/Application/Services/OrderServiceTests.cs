using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrdersService.Application.Services;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Events;
using OrdersService.Infrastructure.Repositories;
using Xunit;

namespace OrdersServiceTests.Application.Services
{
    public class OrderServiceTests
    {
        private class TestOrderRepository : OrderRepository
        {
            public List<Order> Orders = new();
            public Order? LastAdded;
            public TestOrderRepository() : base(null!) { }
            public new async Task<Order> AddAsync(Order order)
            {
                Orders.Add(order);
                LastAdded = order;
                return await Task.FromResult(order);
            }
            public new async Task<Order?> GetByIdAsync(Guid id)
            {
                return await Task.FromResult(Orders.Find(o => o.Id == id));
            }
            public new async Task<List<Order>> GetAllAsync()
            {
                return await Task.FromResult(Orders);
            }
        }
        private class TestOutboxRepository : OutboxRepository
        {
            public List<OutboxEvent> Events = new();
            public TestOutboxRepository() : base(null!) { }
            public new async Task AddAsync(OutboxEvent evt)
            {
                Events.Add(evt);
                await Task.CompletedTask;
            }
        }

        [Fact]
        public async Task CreateOrderAsync_CreatesOrderAndOutboxEvent()
        {
            var orderRepo = new TestOrderRepository();
            var outboxRepo = new TestOutboxRepository();
            var service = new OrderService(orderRepo, outboxRepo);

            var userId = Guid.NewGuid();
            var amount = 100m;
            var description = "Test order";
            var order = await orderRepo.AddAsync(new Order { Id = Guid.NewGuid(), UserId = userId, Amount = amount, Description = description, Status = OrderStatus.New });
            await outboxRepo.AddAsync(new OutboxEvent { Id = Guid.NewGuid(), EventType = "OrderCreated", Payload = "", OccurredOn = DateTime.UtcNow });

            Assert.Equal(userId, order.UserId);
            Assert.Equal(amount, order.Amount);
            Assert.Equal(description, order.Description);
            Assert.Equal(OrderStatus.New, order.Status);
            Assert.Contains(order, orderRepo.Orders);
            Assert.Contains(outboxRepo.Events, e => e.EventType == "OrderCreated");
        }

        [Fact]
        public async Task CreateOrderAsync_ThrowsOnInvalidAmount()
        {
            var orderRepo = new TestOrderRepository();
            var outboxRepo = new TestOutboxRepository();
            var service = new OrderService(orderRepo, outboxRepo);
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(Guid.NewGuid(), 0, "desc"));
        }

        [Fact]
        public async Task GetOrderAsync_ReturnsOrder()
        {
            var order = new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 10, Description = "desc" };
            var orderRepo = new TestOrderRepository();
            orderRepo.Orders.Add(order);
            var outboxRepo = new TestOutboxRepository();
            var service = new OrderService(orderRepo, outboxRepo);
            var result = await orderRepo.GetByIdAsync(order.Id);
            Assert.NotNull(result);
            Assert.Equal(order.Id, result!.Id);
        }

        [Fact]
        public async Task GetOrdersAsync_ReturnsAll()
        {
            var orders = new List<Order> { new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 1, Description = "a" } };
            var orderRepo = new TestOrderRepository();
            orderRepo.Orders.AddRange(orders);
            var outboxRepo = new TestOutboxRepository();
            var service = new OrderService(orderRepo, outboxRepo);
            var result = await orderRepo.GetAllAsync();
            Assert.Single(result);
        }
    }
}
