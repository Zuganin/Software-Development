namespace mini_hw_2.Domain.ValueObjects.FeedingSchedule;

public record FeedingTime
{
    public DateTime Value { get; }

    public FeedingTime(DateTime value)
    {
        Value = value;
    }

    public override string ToString() => Value.ToString("g");
}
