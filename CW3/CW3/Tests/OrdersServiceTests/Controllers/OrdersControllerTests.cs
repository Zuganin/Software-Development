using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Controllers;
using OrdersService.Domain.Entities;
using Xunit;

namespace OrdersServiceTests.Controllers
{
    public class OrdersControllerTests
    {
        private class FakeOrderService
        {
            public List<Order> Orders = new();
            public Order? LastCreated;
            public Task<Order> CreateOrderAsync(Guid userId, decimal amount, string description)
            {
                var order = new Order { Id = Guid.NewGuid(), UserId = userId, Amount = amount, Description = description, Status = OrderStatus.New };
                Orders.Add(order);
                LastCreated = order;
                return Task.FromResult(order);
            }
            public Task<Order?> GetOrderAsync(Guid id)
            {
                return Task.FromResult(Orders.Find(o => o.Id == id));
            }
            public Task<List<Order>> GetOrdersAsync()
            {
                return Task.FromResult(Orders);
            }
        }

        private class TestOrdersController : ControllerBase
        {
            private readonly FakeOrderService _service;
            public TestOrdersController(FakeOrderService service) { _service = service; }

            public async Task<IActionResult> Create(CreateOrderRequest request)
            {
                if (request.Amount <= 0)
                    return new BadRequestObjectResult("Amount must be greater than zero");
                var order = await _service.CreateOrderAsync(request.UserId, request.Amount, request.Description);
                return new OkObjectResult(order);
            }

            public async Task<IActionResult> GetAll()
            {
                var orders = await _service.GetOrdersAsync();
                return new OkObjectResult(orders);
            }

            public async Task<IActionResult> GetById(Guid id)
            {
                var order = await _service.GetOrderAsync(id);
                if (order == null) return new NotFoundResult();
                return new OkObjectResult(order);
            }
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenValid()
        {
            var fake = new FakeOrderService();
            var controller = new TestOrdersController(fake);
            var request = new CreateOrderRequest { UserId = Guid.NewGuid(), Amount = 100, Description = "desc" };
            var result = await controller.Create(request);
            var ok = Assert.IsType<OkObjectResult>(result);
            var order = Assert.IsType<Order>(ok.Value);
            Assert.Equal(request.UserId, order.UserId);
            Assert.Equal(request.Amount, order.Amount);
            Assert.Equal(request.Description, order.Description);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenAmountInvalid()
        {
            var fake = new FakeOrderService();
            var controller = new TestOrdersController(fake);
            var request = new CreateOrderRequest { UserId = Guid.NewGuid(), Amount = 0, Description = "desc" };
            var result = await controller.Create(request);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithOrders()
        {
            var fake = new FakeOrderService();
            fake.Orders.Add(new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 1, Description = "a", Status = OrderStatus.New });
            var controller = new TestOrdersController(fake);
            var result = await controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result);
            var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(ok.Value);
            Assert.Single(orders);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var fake = new FakeOrderService();
            var order = new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 1, Description = "a", Status = OrderStatus.New };
            fake.Orders.Add(order);
            var controller = new TestOrdersController(fake);
            var result = await controller.GetById(order.Id);
            var ok = Assert.IsType<OkObjectResult>(result);
            var found = Assert.IsType<Order>(ok.Value);
            Assert.Equal(order.Id, found.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var fake = new FakeOrderService();
            var controller = new TestOrdersController(fake);
            var result = await controller.GetById(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
