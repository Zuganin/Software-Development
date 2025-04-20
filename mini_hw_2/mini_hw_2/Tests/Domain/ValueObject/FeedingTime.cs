using mini_hw_2.Domain.Entities.FeedingSchedule;
using mini_hw_2.Domain.ValueObjects.FeedingSchedule;

namespace Tests.Domain.ValueObject;

public class FeedingTimeTests
{
    [Fact]
    public void Constructor_ShouldSetValue_WhenTimeIsInFuture()
    {
        var futureTime = DateTime.Now.AddHours(1);
        var feedingTime = new FeedingTime(futureTime);

        Assert.Equal(futureTime, feedingTime.Value);
    }

    
}