using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.Animal;
using mini_hw_2.Domain.Entities.Enclosure;
using mini_hw_2.Domain.Events;

namespace mini_hw_2.Application.Services;

public class AnimalTransferService : IAnimalTransferService
{
    private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository;
        private readonly IEventPublisher _eventPublisher;

        public AnimalTransferService(
            IAnimalRepository animalRepository,
            IEnclosureRepository enclosureRepository,
            IEventPublisher eventPublisher)
        {
            _animalRepository = animalRepository;
            _enclosureRepository = enclosureRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task TransferAnimal(Animal animal, Enclosure NewEnclosure)
        {
            if (animal == null)
                throw new ArgumentException("Животное не найдено");

            var currentEnclosure = await _enclosureRepository.GetByIdAsync(animal.EnclosureId);
            if (currentEnclosure == null)
                throw new ArgumentException("Текущий вольер не найден");

            var targetEnclosure = await _enclosureRepository.GetByIdAsync(NewEnclosure.Id);
            if (targetEnclosure == null)
                throw new ArgumentException("Целевой вольер не найден");

            currentEnclosure.RemoveAnimal(animal);
            targetEnclosure.AddAnimal(animal);
            animal.MoveTo(targetEnclosure.Id);

            await _animalRepository.UpdateAsync(animal);
            await _enclosureRepository.UpdateAsync(currentEnclosure);
            await _enclosureRepository.UpdateAsync(targetEnclosure);
            await _eventPublisher.PublishAsync(new AnimalMovedEvent(animal.Id, targetEnclosure.Id));
        }
        
}