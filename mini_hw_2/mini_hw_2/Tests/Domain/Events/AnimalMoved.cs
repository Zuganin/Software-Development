using mini_hw_2.Domain.Events;

namespace Tests.Domain.Events;

public class AnimalMovedEventTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var animalId = Guid.NewGuid();
        var newEnclosureId = Guid.NewGuid();
        var beforeCreation = DateTime.Now;

        // Act
        var @event = new AnimalMovedEvent(animalId, newEnclosureId);
        var afterCreation = DateTime.Now;

        // Assert
        Assert.Equal(animalId, @event.AnimalId);
        Assert.Equal(newEnclosureId, @event.NewEnclosureId);
        Assert.InRange(@event.OccurredOn, beforeCreation, afterCreation);
    }
}