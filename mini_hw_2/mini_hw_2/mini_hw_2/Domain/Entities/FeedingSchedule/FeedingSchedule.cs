namespace mini_hw_2.Domain.Entities.FeedingSchedule;

public class FeedingSchedule
{
    public Guid Id { get; init; }

    public Guid AnimalId { get; private set; }
    public FeedingTime Time { get; private set; }
    public Food FoodType { get; private set; }
    public bool IsCompleted { get; private set; }

    public FeedingSchedule(Guid id, Guid animalId, FeedingTime time, Food foodType)
    {
        Id = id;
        AnimalId = animalId;
        Time = time;
        FoodType = foodType;
        IsCompleted = false;
    }

    public void Reschedule(FeedingTime newTime)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Нельзя изменить расписание уже выполненного кормления");

        Time = newTime;
    }

    public void MarkAsCompleted()
    {
        if (IsCompleted)
            throw new InvalidOperationException("Кормление уже выполнено");

        IsCompleted = true;
    }
}