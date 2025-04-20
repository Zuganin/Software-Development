using Microsoft.AspNetCore.Mvc;
using mini_hw_2.Application.Interfaces;
using mini_hw_2.Application.Services;
using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.FeedingSchedule;
using mini_hw_2.Domain.ValueObjects.FeedingSchedule;

namespace mini_hw_2.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZooOperationsController : ControllerBase
{
    private readonly IAnimalTransferService _transferService;
    private readonly IFeedingOrganizationService _feedingService;
    private readonly IAnimalRepository _animalRepository;
    private readonly IEnclosureRepository _enclosureRepository;
    private readonly IFeedingScheduleRepository _feedingScheduleRepository;

    public ZooOperationsController(
        IAnimalTransferService transferService,
        IFeedingOrganizationService feedingService,
        IAnimalRepository animalRepository,
        IEnclosureRepository enclosureRepository,
        IFeedingScheduleRepository feedingScheduleRepository)
    {
        _transferService = transferService;
        _feedingService = feedingService;
        _animalRepository = animalRepository;
        _enclosureRepository = enclosureRepository;
        _feedingScheduleRepository = feedingScheduleRepository;
    }

    /// <summary>
    /// Переместить животное в другой вольер
    /// </summary>
    [HttpPost("transfer")]
    public async Task<IActionResult> TransferAnimalAsync([FromBody] AnimalTransferDto dto)
    {
        var animal = await _animalRepository.GetByIdAsync(dto.AnimalId);
        var enclosure = await _enclosureRepository.GetByIdAsync(dto.TargetEnclosureId);

        if (animal == null || enclosure == null)
            return NotFound("Животное или вольер не найдены.");

        await _transferService.TransferAnimal(animal, enclosure);
        return Ok("Животное успешно перемещено.");
    }

    /// <summary>
    /// Добавить запись в расписание кормления
    /// </summary>
    [HttpPost("schedule-feeding")]
    public async Task<IActionResult> ScheduleFeedingAsync([FromBody] FeedingScheduleDto dto)
    {
        var schedule = new FeedingSchedule(
            Guid.NewGuid(),
            dto.AnimalId,
            new FeedingTime(dto.FeedingTime),
            dto.FoodType
        );

        await _feedingService.AddFeedingScheduleAsync(schedule);
        return Ok("Кормление запланировано.");
    }

    /// <summary>
    /// Завершить кормление (отметить как выполненное)
    /// </summary>
    [HttpPost("complete-feeding/{feedingScheduleId}")]
    public async Task<IActionResult> CompleteFeedingAsync(Guid feedingScheduleId)
    {
        await _feedingService.MarkFeedingCompletedAsync(feedingScheduleId);
        return Ok("Кормление завершено.");
    }

    /// <summary>
    /// Получить все расписания кормлений
    /// </summary>
    [HttpGet("feeding-schedules")]
    public async Task<IActionResult> GetFeedingSchedulesAsync()
    {
        var schedules = await _feedingService.GetFeedingSchedulesAsync();
        return Ok(schedules);
    }
}
