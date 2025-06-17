using System;
using Xunit;
using PaymentsService.Domain.Events;

namespace PaymentsServiceTests.Domain.Events
{
    public class InboxEventTests
    {
        [Fact]
        public void InboxEvent_Properties_AreSet()
        {
            var now = DateTime.UtcNow;
            var evt = new InboxEvent
            {
                Id = Guid.NewGuid(),
                EventType = "OrderCreated",
                Payload = "{orderId:1}",
                Status = "Pending",
                ReceivedAt = now,
                ProcessedAt = null,
                Error = null
            };
            Assert.Equal("OrderCreated", evt.EventType);
            Assert.Equal("{orderId:1}", evt.Payload);
            Assert.Equal("Pending", evt.Status);
            Assert.Equal(now, evt.ReceivedAt);
            Assert.Null(evt.ProcessedAt);
            Assert.Null(evt.Error);
        }
    }
}
