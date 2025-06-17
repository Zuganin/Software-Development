using System;

namespace PaymentsService.Domain.Model.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public string Type { get; set; } = string.Empty; // deposit/withdraw
        public string? Description { get; set; }
        public string? IdempotencyKey { get; set; } // Для защиты от двойного списания
    }
}
