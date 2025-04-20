using mini_hw_2.Domain.Entities;

namespace Tests.Domain.Entities;

public class AnimalTests
    {
        [Fact]
        public void Constructor_ShouldInitializeAnimalCorrectly()
        {
            // Arrange
            var name = "Leo";
            var gender = Gender.Male;
            var species = Species.Predators;
            var enclosureType = EnclosureType.ForPredators;
            var enclosureId = Guid.NewGuid();

            // Act
            var animal = new mini_hw_2.Domain.Entities.Animal.Animal(name, gender, species, enclosureType, enclosureId);

            // Assert
            Assert.Equal(name, animal.Name);
            Assert.Equal(gender, animal.Gender);
            Assert.Equal(species, animal.Species);
            Assert.Equal(enclosureType, animal.EnclosureType);
            Assert.Equal(enclosureId, animal.EnclosureId);
            Assert.NotEqual(Guid.Empty, animal.Id);
            Assert.Equal(HealtStatus.Healthy, animal.HealtStatus);
            Assert.Equal(NutritionStatus.Eaten, animal.NutritionStatus);
            Assert.True((DateTime.Now - animal.BirthDate).TotalSeconds < 2);
            Assert.True((DateTime.Now - animal.LastFedTime).TotalSeconds < 2);
        }

        [Fact]
        public void Feed_ShouldUpdateNutritionStatusAndLastFedTime()
        {
            // Arrange
            var animal = CreateDefaultAnimal();
            var originalLastFedTime = animal.LastFedTime;

            // Act
            System.Threading.Thread.Sleep(1000); // Ждём, чтобы время обновилось
            animal.Feed();

            // Assert
            Assert.Equal(NutritionStatus.Eaten, animal.NutritionStatus);
            Assert.True((bool)(animal.LastFedTime > originalLastFedTime));
        }

        [Fact]
        public void Heal_ShouldUpdateHealthStatus()
        {
            // Arrange
            var animal = CreateDefaultAnimal();

           
            typeof(mini_hw_2.Domain.Entities.Animal.Animal).GetProperty(nameof(mini_hw_2.Domain.Entities.Animal.Animal.HealtStatus))!
                .SetValue(animal, HealtStatus.Sick);

            // Act
            animal.Heal();

            // Assert
            Assert.Equal(HealtStatus.Healthy, animal.HealtStatus);
        }

        [Fact]
        public void MoveTo_ShouldChangeEnclosureId()
        {
            // Arrange
            var animal = CreateDefaultAnimal();
            var newEnclosureId = Guid.NewGuid();

            // Act
            animal.MoveTo(newEnclosureId);

            // Assert
            Assert.Equal(newEnclosureId, animal.EnclosureId);
        }

        private mini_hw_2.Domain.Entities.Animal.Animal CreateDefaultAnimal()
        {
            return new mini_hw_2.Domain.Entities.Animal.Animal("Test", Gender.Female, Species.Predators, EnclosureType.ForPredators, Guid.NewGuid());
        }
    }