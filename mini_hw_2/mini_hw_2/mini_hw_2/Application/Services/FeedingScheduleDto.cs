using mini_hw_2.Domain.Entities;

namespace mini_hw_2.Application.Services;

public class FeedingScheduleDto
{
    public Guid AnimalId { get; set; }
    public DateTime FeedingTime { get; set; }
    public Food FoodType { get; set; }
}