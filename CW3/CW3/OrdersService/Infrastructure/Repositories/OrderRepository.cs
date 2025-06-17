using OrdersService.Domain.Entities;
using OrdersService.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace OrdersService.Infrastructure.Repositories
{
    public class OrderRepository
    {
        private readonly OrdersDbContext _db;
        public OrderRepository(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<Order> AddAsync(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _db.Orders.FindAsync(id);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _db.Orders.ToListAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
        }
    }
}
