using Microsoft.Extensions.Diagnostics.HealthChecks;
using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.Animal;
using mini_hw_2.Domain.Entities.Enclosure;

namespace mini_hw_2.Application.Services;
public class ZooStatisticsService : IStatisticsService
{
    private readonly IRepository<Animal> _animalRepository;
    private readonly IRepository<Enclosure> _enclosureRepository;

    public ZooStatisticsService(
        IRepository<Animal> animalRepository,
        IRepository<Enclosure> enclosureRepository)
    {
        _animalRepository = animalRepository;
        _enclosureRepository = enclosureRepository;
    }

    public async Task<ZooStatisticsDto> CollectStatisticsAsync()
    {
        var animals = (await _animalRepository.GetAllAsync()).ToList();
        var enclosures = (await _enclosureRepository.GetAllAsync()).ToList();

        int totalAnimals = animals.Count;
        int totalEnclosures = enclosures.Count;


        var occupiedEnclosureIds = animals.Select(a => a.EnclosureId).Distinct().ToHashSet();
        int occupiedEnclosures = enclosures.Count(e => occupiedEnclosureIds.Contains(e.Id));
        int freeEnclosures = totalEnclosures - occupiedEnclosures;

        int sickAnimals = animals.Count(a => a.HealtStatus == HealtStatus.Sick);

        return new ZooStatisticsDto
        {
            TotalAnimals = totalAnimals,
            TotalEnclosures = totalEnclosures,
            OccupiedEnclosures = occupiedEnclosures,
            FreeEnclosures = freeEnclosures,
            SickAnimals = sickAnimals
        };
    }
}