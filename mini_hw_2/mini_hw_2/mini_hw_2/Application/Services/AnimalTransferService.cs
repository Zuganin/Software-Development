using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.Enclosure;

namespace mini_hw_2.Application.Services;

public class AnimalTransferService : IAnimalTransferService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IEnclosureRepository _enclosureRepository;
    private readonly IEventPublisher _eventPublisher;
    
    public void TransferAnimal(Animal animal, Enclosure NewEnclosure)
    {
        if(NewEnclosure.CurrentAnimalCount == 0)
        {
            NewEnclosure.Clean();
            NewEnclosure.AddAnimal(animal);
        }
        else
        {
            NewEnclosure.AddAnimal(animal);
        }
        Enclosure previousEnclosure = IEclousureRepository.GetEnclosureById(animal.EnclosureId);
    }
}