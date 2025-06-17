using System;

namespace PaymentsService.Domain.Events
{
    public class OutboxEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EventType { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public bool Processed { get; set; } = false;
        public DateTime? ProcessedOn { get; set; }
        public string? CorrelationId { get; set; } // Для идемпотентности
    }
}
