using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PaymentsService.Domain.Events;

namespace PaymentsService.Domain.Model.Interfaces
{
    public interface IInboxRepository
    {
        Task AddAsync(InboxEvent inboxEvent, CancellationToken cancellationToken);
        Task<InboxEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<InboxEvent>> GetPendingAsync(CancellationToken cancellationToken);
        Task UpdateAsync(InboxEvent inboxEvent, CancellationToken cancellationToken);
    }
}
