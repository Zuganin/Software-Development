using Microsoft.AspNetCore.Mvc;
using mini_hw_2.Application.Interfaces;

namespace mini_hw_2.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet]
    public IActionResult GetZooStatistics()
    {
        return Ok(_statisticsService.CollectStatisticsAsync());
    }
}
