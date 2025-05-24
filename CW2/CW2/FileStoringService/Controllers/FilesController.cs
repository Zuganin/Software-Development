using Microsoft.AspNetCore.Mvc;
using FileStoringService.Domain.Interfaces;
using FileStoringService.Application.DTOs;
using FileStoringService.Application.Interfaces;

namespace FileStoringService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileStoringService _service;

    public FilesController(IFileStoringService service)
    {
        _service = service;
    }

    // POST api/files
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var tempPath = Path.GetTempFileName();
        await using (var stream = System.IO.File.Create(tempPath))
        {
            await file.CopyToAsync(stream, ct);
        }

        var dto = await _service.SaveFileAsync(tempPath, ct);
        return CreatedAtAction(nameof(GetMetadata), new { id = dto.Id }, dto);
    }

    // GET api/files
    [HttpGet]
    public async Task<IActionResult> GetAllMetadata(CancellationToken ct)
    {
        var list = await _service.GetAllFilesMetadataAsync(ct);
        return Ok(list);
    }

    // GET api/files/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        try
        {
            var (content, name) = await _service.GetFileAsync(id, ct);
            return File(content, "application/octet-stream", name);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // GET api/files/{id}/metadata
    [HttpGet("{id:guid}/metadata")]
    public async Task<IActionResult> GetMetadata(Guid id, CancellationToken ct)
    {
        try
        {
            var meta = await _service.GetFileMetadataAsync(id, ct);
            return Ok(meta);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE api/files/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var removed = await _service.DeleteFileAsync(id, ct);
        return removed ? NoContent() : NotFound();
    }
}
