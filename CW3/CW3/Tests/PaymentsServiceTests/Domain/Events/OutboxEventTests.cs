using System;
using Xunit;
using PaymentsService.Domain.Events;

namespace PaymentsServiceTests.Domain.Events
{
    public class OutboxEventTests
    {
        [Fact]
        public void OutboxEvent_Properties_AreSet()
        {
            var evt = new OutboxEvent
            {
                Id = Guid.NewGuid(),
                EventType = "Test",
                Payload = "{}",
                OccurredOn = DateTime.UtcNow,
                Processed = false
            };
            Assert.Equal("Test", evt.EventType);
            Assert.Equal("{}", evt.Payload);
            Assert.False(evt.Processed);
        }
    }
}