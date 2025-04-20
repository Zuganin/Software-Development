using Microsoft.AspNetCore.Mvc;
using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Entities.Enclosure;
using mini_hw_2.Presentation.Requests;

namespace mini_hw_2.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnclosuresController : ControllerBase
{
    private readonly IRepository<Enclosure> _enclosureRepository;

    public EnclosuresController(IRepository<Enclosure> enclosureRepository)
    {
        _enclosureRepository = enclosureRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var enclosures = await _enclosureRepository.GetAllAsync();
        return Ok(enclosures);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var enclosure = await _enclosureRepository.GetByIdAsync(id);
        return enclosure == null ? NotFound() : Ok(enclosure);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnclosureRequest request)
    {
        var enclosure = new Enclosure(Guid.NewGuid(), request.Type, request.Size, request.MaxCapacity);
        await _enclosureRepository.AddAsync(enclosure);
        return CreatedAtAction(nameof(GetById), new { id = enclosure.Id }, enclosure);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var enclosure = await _enclosureRepository.GetByIdAsync(id);
        if (enclosure == null)
            return NotFound();

        await _enclosureRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/clean")]
    public async Task<IActionResult> Clean(Guid id)
    {
        var enclosure = await _enclosureRepository.GetByIdAsync(id);
        if (enclosure == null)
            return NotFound();

        enclosure.Clean();
        await _enclosureRepository.UpdateAsync(enclosure);
        return Ok("Enclosure cleaned.");
    }
}