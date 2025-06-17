using PaymentsService.Domain.Model.Entities;

namespace Test.PaymentsServiceTests.Domain.Model.Entities
{
    public class TransactionTests
    {
        [Fact]
        public void Transaction_Properties_AreSet()
        {
            // Arrange
            var id = Guid.NewGuid();
            var accId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            // Act
            var t = new Transaction
            {
                Id = id,
                AccountId = accId,
                Amount = 123,
                Type = "deposit",
                OccurredOn = now
            };

            // Assert
            Assert.Equal(id, t.Id);
            Assert.Equal(accId, t.AccountId);
            Assert.Equal(123, t.Amount);
            Assert.Equal("deposit", t.Type);
            Assert.Equal(now, t.OccurredOn);
        }
    }
}
