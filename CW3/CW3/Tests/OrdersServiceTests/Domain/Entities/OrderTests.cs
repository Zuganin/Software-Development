using System;
using Xunit;
using OrdersService.Domain.Entities;

namespace OrdersServiceTests.Domain.Entities
{
    public class OrderTests
    {
        [Fact]
        public void Order_InitializesWithDefaults()
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Amount = 100,
                Description = "Test order"
            };
            Assert.Equal(OrderStatus.New, order.Status);
            Assert.Equal(100, order.Amount);
            Assert.Equal("Test order", order.Description);
        }

        [Fact]
        public void CanSetOrderStatus()
        {
            var order = new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 50 };
            order.Status = OrderStatus.Finished;
            Assert.Equal(OrderStatus.Finished, order.Status);
            order.Status = OrderStatus.Cancelled;
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }
    }
}
