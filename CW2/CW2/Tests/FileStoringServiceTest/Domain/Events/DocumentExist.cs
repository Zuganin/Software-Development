using FileStoringService.Entities.Events;

namespace FileStoringService.Domain.Events
{
    public class DocumentExistTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var existingDocumentId = Guid.NewGuid();
            var documentHash = "testhash";
            var attemptedFileName = "file.txt";

            // Act
            var evt = new DocumentExist(existingDocumentId, documentHash, attemptedFileName);

            // Assert
            Assert.Equal(existingDocumentId, evt.ExistingDocumentId);
            Assert.Equal(documentHash, evt.DocumentHash);
            Assert.Equal(attemptedFileName, evt.AttemptedFileName);
            Assert.NotEqual(Guid.Empty, evt.Id);
            Assert.True((DateTime.UtcNow - evt.OccurredOn).TotalSeconds < 5);
        }
    }
}