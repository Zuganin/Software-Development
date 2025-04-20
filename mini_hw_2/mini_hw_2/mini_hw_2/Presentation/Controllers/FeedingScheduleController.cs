using Microsoft.AspNetCore.Mvc;
using mini_hw_2.Application.Interfaces;
using mini_hw_2.Application.Services;
using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.FeedingSchedule;
using mini_hw_2.Domain.ValueObjects.FeedingSchedule;

namespace mini_hw_2.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedingScheduleController : ControllerBase
{
    private readonly IFeedingOrganizationService _feedingService;

    public FeedingScheduleController(IFeedingOrganizationService feedingService)
    {
        _feedingService = feedingService;
    }

    [HttpPost("schedule")]
    public async Task<IActionResult> AddFeedingSchedule([FromBody] FeedingScheduleDto dto)
    {
        var schedule = new FeedingSchedule(Guid.NewGuid(), dto.AnimalId, new FeedingTime(dto.FeedingTime), dto.FoodType );
        await _feedingService.AddFeedingScheduleAsync(schedule);
        return Ok(schedule);
    }

    [HttpPut("reschedule/{id}")]
    public async Task<IActionResult> RescheduleFeeding(Guid id, [FromBody] DateTime newTime)
    {
        await _feedingService.RescheduleFeedingAsync(id, new FeedingTime(newTime));
        return Ok();
    }

    [HttpPut("complete/{id}")]
    public async Task<IActionResult> MarkCompleted(Guid id)
    {
        await _feedingService.MarkFeedingCompletedAsync(id);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schedules = await _feedingService.GetFeedingSchedulesAsync();
        return Ok(schedules);
    }

    [HttpPost("check-times")]
    public async Task<IActionResult> CheckFeedingTimes()
    {
        await _feedingService.CheckFeedingTimeAsync();
        return Ok();
    }
}
