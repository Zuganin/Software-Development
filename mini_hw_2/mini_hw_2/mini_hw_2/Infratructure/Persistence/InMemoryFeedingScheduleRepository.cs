using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Entities.FeedingSchedule;

namespace mini_hw_2.Infratructure.Persistence;

public class InMemoryFeedingScheduleRepository : InMemoryRepository<FeedingSchedule>, IFeedingScheduleRepository;
