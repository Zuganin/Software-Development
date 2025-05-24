using FileAnalysisService.Application.DTOs;
using FileAnalysisService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileAnalysisService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyzeController : ControllerBase
{
    private readonly AnalyzeFileService _service;

    public AnalyzeController(AnalyzeFileService service)
    {
        _service = service;
    }

    [HttpGet("{fileId:guid}")]
    public async Task<IActionResult> GetAsync(Guid fileId, CancellationToken ct)
    {
        try
        {
            var response = await _service.AnalyzeAsync(new AnalyzeFileRequest { FileId = fileId }, ct);
            return Ok(response);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }
}