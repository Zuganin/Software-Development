using FileAnalysisService.Application.DTOs;
using FileAnalysisService.Application.Interfaces;
using FileAnalysisService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileAnalysisService.Controllers;

[ApiController]
[Route("api/analyze")]
public class AnalyzeController : ControllerBase
{
    private readonly IAnalyzeFileService _service;
    

    public AnalyzeController(IAnalyzeFileService service)
    {
        _service = service;
    }

    [HttpGet]
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
    // GET api/analyze/wordcloud/{fileId}
    [HttpGet("wordcloud/{fileId}")]
    public async Task<IActionResult> DownloadWordCloud(Guid fileId)
    {
        try
        {
            var (content, fileName) = await _service.DownloadWordCloudAsync(fileId);

            // Возвращаем файл как response с типом image/png
            return File(content, "image/png", fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            // _logger.LogError(ex, "Error downloading word cloud");
            return StatusCode(500, "Internal server error");
        }
    }
}