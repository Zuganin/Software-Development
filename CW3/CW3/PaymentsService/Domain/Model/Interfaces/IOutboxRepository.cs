using System;
using System.Threading;
using System.Threading.Tasks;
using PaymentsService.Domain.Events;

namespace PaymentsService.Domain.Model.Interfaces
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken);
        Task MarkAsProcessedAsync(Guid eventId, CancellationToken cancellationToken);
        // Можно добавить методы для выборки непроцессенных событий
    }
}
