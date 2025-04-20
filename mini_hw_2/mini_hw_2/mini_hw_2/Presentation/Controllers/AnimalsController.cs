using Microsoft.AspNetCore.Mvc;
using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Entities.Animal;
using mini_hw_2.Domain.Entities.Enclosure;
using mini_hw_2.Presentation.Requests;

namespace mini_hw_2.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IRepository<Animal> _animalRepository;
    private readonly IRepository<Enclosure> _enclosureRepository;
    private readonly IAnimalTransferService _transferService;

    public AnimalsController(
        IRepository<Animal> animalRepository,
        IRepository<Enclosure> enclosureRepository,
        IAnimalTransferService transferService)
    {
        _animalRepository = animalRepository;
        _enclosureRepository = enclosureRepository;
        _transferService = transferService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var animals = await _animalRepository.GetAllAsync();
        return Ok(animals);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var animal = await _animalRepository.GetByIdAsync(id);
        return animal == null ? NotFound() : Ok(animal);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnimalRequest request)
    {
        var enclosure = await _enclosureRepository.GetByIdAsync(request.EnclosureId);
        if (enclosure == null)
            return BadRequest("Enclosure not found");

        var animal = new Animal(request.Name, request.Gender, request.Species, request.EnclosureType, request.EnclosureId);
        await _animalRepository.AddAsync(animal);

        enclosure.AddAnimal(animal);
        await _enclosureRepository.UpdateAsync(enclosure);

        return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var animal = await _animalRepository.GetByIdAsync(id);
        if (animal == null)
            return NotFound();

        var enclosure = await _enclosureRepository.GetByIdAsync(animal.EnclosureId);
        if (enclosure != null)
        {
            enclosure.RemoveAnimal(animal);
            await _enclosureRepository.UpdateAsync(enclosure);
        }

        await _animalRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/transfer")]
    public async Task<IActionResult> Transfer(Guid id, [FromBody] Guid targetEnclosureId)
    {
        var animal = await _animalRepository.GetByIdAsync(id);
        var enclosure = await _enclosureRepository.GetByIdAsync(targetEnclosureId);

        if (animal == null || enclosure == null)
            return BadRequest("Animal or enclosure not found");

        await _transferService.TransferAnimal(animal, enclosure);
        return Ok("Animal transferred successfully");
    }
}