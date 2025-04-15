namespace mini_hw_2.Domain.Entities.FeedingSchedule;

public record FeedingTime
{
    public DateTime Value { get; }

    public FeedingTime(DateTime value)
    {
        if (value < DateTime.Now)
            throw new ArgumentException("Время кормления не может быть в прошлом");
        Value = value;
    }

    public override string ToString() => Value.ToString("g");
}
