using mini_hw_2.Domain.Entities.FeedingSchedule;

namespace mini_hw_2.Application.Interfaces;

public interface IFeedingOrganizationService
{
   
    public Task AddFeedingScheduleAsync(FeedingSchedule schedule);
    
    public Task RescheduleFeedingAsync(Guid feedingScheduleId, FeedingTime newTime);
    
    public Task MarkFeedingCompletedAsync(Guid feedingScheduleId);
    
    public Task<IEnumerable<FeedingSchedule>> GetFeedingSchedulesAsync();

    public Task CheckFeedingTimeAsync();
}