using mini_hw_2.Application.Interfaces;
using mini_hw_2.Application.Services;
using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.Animal;
using mini_hw_2.Domain.Entities.Enclosure;
using mini_hw_2.Domain.Events;
using Moq;
using Xunit;

namespace Tests.Application;

public class AnimalTransferServiceTests
    {
        private readonly Mock<IAnimalRepository> _animalRepoMock = new();
        private readonly Mock<IEnclosureRepository> _enclosureRepoMock = new();
        private readonly Mock<IEventPublisher> _eventPublisherMock = new();

        private AnimalTransferService CreateService() =>
            new(_animalRepoMock.Object, _enclosureRepoMock.Object, _eventPublisherMock.Object);

        [Fact]
        public async Task TransferAnimal_ShouldTransferAnimalSuccessfully()
        {
            // Arrange
            var animal = new Animal("Лев", Gender.Male, Species.Predators, EnclosureType.ForPredators, Guid.NewGuid());
            var oldEnclosure = new Enclosure(animal.EnclosureId, EnclosureType.ForPredators, new SquareMeters(100), 10);
            var newEnclosure = new Enclosure(Guid.NewGuid(), EnclosureType.ForPredators, new SquareMeters(100), 10);

            oldEnclosure.AddAnimal(animal);

            _enclosureRepoMock.Setup(r => r.GetByIdAsync(animal.EnclosureId)).ReturnsAsync(oldEnclosure);
            _enclosureRepoMock.Setup(r => r.GetByIdAsync(newEnclosure.Id)).ReturnsAsync(newEnclosure);

            var service = CreateService();

            // Act
            await service.TransferAnimal(animal, newEnclosure);

            // Assert
            Assert.Equal(newEnclosure.Id, animal.EnclosureId);
            Assert.Contains(animal, newEnclosure.Animals);
            Assert.DoesNotContain(animal, oldEnclosure.Animals);
            _animalRepoMock.Verify(r => r.UpdateAsync(animal), Times.Once);
            _enclosureRepoMock.Verify(r => r.UpdateAsync(oldEnclosure), Times.Once);
            _enclosureRepoMock.Verify(r => r.UpdateAsync(newEnclosure), Times.Once);
            _eventPublisherMock.Verify(e => e.PublishAsync(It.IsAny<AnimalMovedEvent>()), Times.Once);
        }

        [Fact]
        public async Task TransferAnimal_ShouldThrow_WhenAnimalIsNull()
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.TransferAnimal(null, new Enclosure(Guid.NewGuid(), EnclosureType.ForPredators, new SquareMeters(50), 5)));
            Assert.Equal("Животное не найдено", ex.Message);
        }

        [Fact]
        public async Task TransferAnimal_ShouldThrow_WhenCurrentEnclosureNotFound()
        {
            // Arrange
            var animal = new Animal("Зебра", Gender.Female, Species.Herbivores, EnclosureType.ForHerbivores, Guid.NewGuid());

            _enclosureRepoMock.Setup(r => r.GetByIdAsync(animal.EnclosureId)).ReturnsAsync((Enclosure)null);

            var service = CreateService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.TransferAnimal(animal, new Enclosure(Guid.NewGuid(), EnclosureType.ForHerbivores, new SquareMeters(50), 5)));
            Assert.Equal("Текущий вольер не найден", ex.Message);
        }

        [Fact]
        public async Task TransferAnimal_ShouldThrow_WhenTargetEnclosureNotFound()
        {
            // Arrange
            var animal = new Animal("Попугай", Gender.Male, Species.Birds, EnclosureType.ForBirds, Guid.NewGuid());
            var currentEnclosure = new Enclosure(animal.EnclosureId, EnclosureType.ForBirds, new SquareMeters(30), 5);

            _enclosureRepoMock.Setup(r => r.GetByIdAsync(animal.EnclosureId)).ReturnsAsync(currentEnclosure);
            _enclosureRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Enclosure)null);

            var service = CreateService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.TransferAnimal(animal, new Enclosure(Guid.NewGuid(), EnclosureType.ForPredators, new SquareMeters(50), 5)));
            Assert.Equal("Текущий вольер не найден", ex.Message);
        }
    }