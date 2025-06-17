using OrdersService.Domain.Events;
using OrdersService.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace OrdersService.Infrastructure.Repositories
{
    public class OutboxRepository
    {
        private readonly OrdersDbContext _db;
        public OutboxRepository(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(OutboxEvent evt)
        {
            _db.OutboxEvents.Add(evt);
            await _db.SaveChangesAsync();
        }

        public async Task<List<OutboxEvent>> GetUnprocessedAsync(int take = 10)
        {
            return await _db.OutboxEvents
                .Where(e => !e.Processed)
                .OrderBy(e => e.OccurredOn)
                .Take(take)
                .ToListAsync();
        }

        public async Task MarkProcessedAsync(OutboxEvent evt)
        {
            evt.Processed = true;
            evt.ProcessedOn = DateTime.UtcNow;
            _db.OutboxEvents.Update(evt);
            await _db.SaveChangesAsync();
        }
    }
}
