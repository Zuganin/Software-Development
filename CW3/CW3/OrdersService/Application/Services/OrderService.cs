using OrdersService.Domain.Entities;
using OrdersService.Infrastructure.Repositories;
using OrdersService.Domain.Events;
using System.Text.Json;

namespace OrdersService.Application.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly OutboxRepository _outboxRepository;

        public OrderService(OrderRepository orderRepository, OutboxRepository outboxRepository)
        {
            _orderRepository = orderRepository;
            _outboxRepository = outboxRepository;
        }

        public async Task<Order> CreateOrderAsync(Guid userId, decimal amount, string description)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero");
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = amount,
                Description = description,
                Status = OrderStatus.New
            };
            await _orderRepository.AddAsync(order);
            var evt = new OutboxEvent
            {
                Id = Guid.NewGuid(),
                EventType = "OrderCreated",
                Payload = JsonSerializer.Serialize(order),
                OccurredOn = DateTime.UtcNow
            };
            await _outboxRepository.AddAsync(evt);
            return order;
        }

        public Task<Order?> GetOrderAsync(Guid id) => _orderRepository.GetByIdAsync(id);
        public Task<List<Order>> GetOrdersAsync() => _orderRepository.GetAllAsync();
    }
}
