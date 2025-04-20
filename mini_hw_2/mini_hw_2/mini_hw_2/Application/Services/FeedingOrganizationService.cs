using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Entities.FeedingSchedule;
using mini_hw_2.Domain.Events;
using mini_hw_2.Domain.ValueObjects.FeedingSchedule;

namespace mini_hw_2.Application.Services;

public class FeedingOrganizationService : IFeedingOrganizationService
{
    private readonly IFeedingScheduleRepository _feedingScheduleRepository;
    private readonly IEventPublisher _eventPublisher;

    public FeedingOrganizationService(
        IFeedingScheduleRepository feedingScheduleRepository,
        IEventPublisher eventPublisher)
    {
        _feedingScheduleRepository = feedingScheduleRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task AddFeedingScheduleAsync(FeedingSchedule schedule)
    {
        await _feedingScheduleRepository.AddAsync(schedule);
        
    }
    
    public async Task RescheduleFeedingAsync(Guid feedingScheduleId, FeedingTime newTime)
    {
        var schedule = await _feedingScheduleRepository.GetByIdAsync(feedingScheduleId);
        if (schedule == null)
            throw new ArgumentException("Расписание кормления не найдено", nameof(feedingScheduleId));

        schedule.Reschedule(newTime);
        await _feedingScheduleRepository.UpdateAsync(schedule);
    }

    public async Task MarkFeedingCompletedAsync(Guid feedingScheduleId)
    {
        var schedule = await _feedingScheduleRepository.GetByIdAsync(feedingScheduleId);
        if (schedule == null)
            throw new ArgumentException("Расписание кормления не найдено", nameof(feedingScheduleId));

        schedule.MarkAsCompleted();
        await _feedingScheduleRepository.UpdateAsync(schedule);
    }

    public async Task<IEnumerable<FeedingSchedule>> GetFeedingSchedulesAsync()
    {
        return await _feedingScheduleRepository.GetAllAsync();
    }

    public async Task CheckFeedingTimeAsync()
    {
        var schedules = await _feedingScheduleRepository.GetAllAsync();
        foreach (var schedule in schedules)
        {
            if (schedule.Time.Value <= DateTime.Now && !schedule.IsCompleted)
            {
                await _eventPublisher.PublishAsync(
                    new FeedingTimeEvent(schedule.Id, schedule.Time.Value, schedule.FoodType));
            }
        }
    }

}