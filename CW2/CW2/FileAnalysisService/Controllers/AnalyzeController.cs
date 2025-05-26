using FileAnalysisService.Application.DTOs;
using FileAnalysisService.Application.Interfaces;
using FileAnalysisService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileAnalysisService.Controllers;

[ApiController]
[Route("[controller]")]
public class AnalyzeController : ControllerBase
{
    private readonly IAnalyzeFileService _service;

    public AnalyzeController(IAnalyzeFileService service)
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