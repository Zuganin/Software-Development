using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Events;

namespace Tests.Domain.Events;

public class FeedingTimeEventTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();
        var feedingTime = DateTime.Now.AddHours(1);
        var food = Food.Meat;
        var beforeCreation = DateTime.Now;

        // Act
        var @event = new FeedingTimeEvent(scheduleId, feedingTime, food);
        var afterCreation = DateTime.Now;

        // Assert
        Assert.Equal(scheduleId, @event.FeedingScheduleId);
        Assert.Equal(feedingTime, @event.FeedingTime);
        Assert.Equal(food, @event.FoodType);
        Assert.InRange(@event.OccurredOn, beforeCreation, afterCreation);
    }

    [Fact]
    public void FeedingTimeEvent_ShouldImplementIDomainEvent()
    {
        // Arrange & Act
        var @event = new FeedingTimeEvent(Guid.NewGuid(), DateTime.Now.AddMinutes(5), Food.Grass);

        // Assert
        Assert.IsAssignableFrom<IDomainEvent>(@event);
    }
}