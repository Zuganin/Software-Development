using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Domain.Events;
using PaymentsService.Domain.Model.Interfaces;
using PaymentsService.Infrastructure.DbContext;

namespace PaymentsService.Infrastructure.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly PaymentsDbContext _dbContext;
        public OutboxRepository(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken)
        {
            _dbContext.OutboxEvents.Add(outboxEvent);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task MarkAsProcessedAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var evt = await _dbContext.OutboxEvents.FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);
            if (evt != null)
            {
                evt.Processed = true;
                evt.ProcessedOn = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        // Можно добавить методы для выборки непроцессенных событий
    }
}
