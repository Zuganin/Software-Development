using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Domain.Events;
using PaymentsService.Domain.Model.Interfaces;
using PaymentsService.Infrastructure.DbContext;

namespace PaymentsService.Infrastructure.Repositories
{
    public class InboxRepository : IInboxRepository
    {
        private readonly PaymentsDbContext _context;

        public InboxRepository(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InboxEvent inboxEvent, CancellationToken cancellationToken)
        {
            await _context.InboxEvents.AddAsync(inboxEvent, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<InboxEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.InboxEvents.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<List<InboxEvent>> GetPendingAsync(CancellationToken cancellationToken)
        {
            return await _context.InboxEvents
                .Where(e => e.Status == "Pending")
                .OrderBy(e => e.ReceivedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(InboxEvent inboxEvent, CancellationToken cancellationToken)
        {
            _context.InboxEvents.Update(inboxEvent);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
