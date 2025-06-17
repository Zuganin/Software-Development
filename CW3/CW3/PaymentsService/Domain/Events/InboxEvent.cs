using System;

namespace PaymentsService.Domain.Events
{
    public class InboxEvent
    {
        public Guid Id { get; set; } // Id события (например, из Kafka)
        public string EventType { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public string Status { get; set; } = "Pending"; // Pending, Processed, Error
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public string? Error { get; set; }
    }
}
