using mini_hw_2.Domain.Entities;

namespace mini_hw_2.Domain.Events;

public class FeedingTimeEvent : IDomainEvent
{
    public Guid FeedingScheduleId { get; }
    public DateTime FeedingTime { get; }
    public Food FoodType { get; }
    public DateTime OccurredOn { get; }

    public FeedingTimeEvent(Guid feedingScheduleId, DateTime feedingTime, Food food)
    {
        FeedingScheduleId = feedingScheduleId;
        FeedingTime = feedingTime;
        FoodType = food;
        OccurredOn = DateTime.Now;
    }
}