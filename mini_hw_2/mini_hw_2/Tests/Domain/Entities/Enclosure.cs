using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.Animal;
using mini_hw_2.Domain.Entities.Enclosure;

namespace Tests.Domain.Entities;

public class EnclosureTests
    {
        [Fact]
        public void Constructor_ShouldInitializeEnclosureCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var type = EnclosureType.ForPredators;
            var size = new SquareMeters(100);
            var capacity = 5;

            // Act
            var enclosure = new Enclosure(id, type, size, capacity);

            // Assert
            Assert.Equal(id, enclosure.Id);
            Assert.Equal(type, enclosure.Type);
            Assert.Equal(size, enclosure.Size);
            Assert.Equal(capacity, enclosure.MaxCapacity);
            Assert.Empty(enclosure.Animals);
        }

        [Fact]
        public void AddAnimal_ShouldIncreaseAnimalCount()
        {
            var enclosure = CreateDefaultEnclosure();
            var animal = CreateTestAnimal();

            enclosure.AddAnimal(animal);

            Assert.Single(enclosure.Animals);
            Assert.Equal(1, enclosure.CurrentAnimalCount);
        }

        [Fact]
        public void AddAnimal_ShouldThrow_WhenEnclosureIsFull()
        {
            var enclosure = new Enclosure(Guid.NewGuid(), EnclosureType.Aquarium, new SquareMeters(50), 1);
            var animal1 = CreateTestAnimal();
            var animal2 = CreateTestAnimal();

            enclosure.AddAnimal(animal1);

            var exception = Assert.Throws<InvalidOperationException>(() => enclosure.AddAnimal(animal2));
            Assert.Equal("Вольер переполнен", exception.Message);
        }

        [Fact]
        public void RemoveAnimal_ShouldDecreaseAnimalCount()
        {
            var enclosure = CreateDefaultEnclosure();
            var animal = CreateTestAnimal();

            enclosure.AddAnimal(animal);
            enclosure.RemoveAnimal(animal);

            Assert.Empty(enclosure.Animals);
            Assert.Equal(0, enclosure.CurrentAnimalCount);
        }

        [Fact]
        public void RemoveAnimal_ShouldThrow_WhenAnimalNotFound()
        {
            var enclosure = CreateDefaultEnclosure();
            var animal = CreateTestAnimal();

            var exception = Assert.Throws<InvalidOperationException>(() => enclosure.RemoveAnimal(animal));
            Assert.Equal("Животное не найдено в вольере", exception.Message);
        }

        [Fact]
        public void Clean_ShouldPrintMessage()
        {
            var enclosure = CreateDefaultEnclosure();

            using var sw = new System.IO.StringWriter();
            Console.SetOut(sw);

            enclosure.Clean();

            var output = sw.ToString().Trim();
            Assert.Equal("Уборка вольера проведена", output);
        }

        private Enclosure CreateDefaultEnclosure()
        {
            return new Enclosure(Guid.NewGuid(), EnclosureType.ForPredators, new SquareMeters(100), 3);
        }

        private Animal CreateTestAnimal()
        {
            return new Animal("Simba", Gender.Male, Species.Predators, EnclosureType.ForPredators, Guid.NewGuid());
        }
    }