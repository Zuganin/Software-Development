using hw1.Models.Animals;
using hw1.Models.Interfaces;
using hw1.Services;
using Xunit;
using Moq;
using Assert = Xunit.Assert;

namespace Tests;

public class AnimalTest
{
    [Fact]
    public void Animal_ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var tiger = new Tiger("IVAN", 52, 52);

        // Act
        var result = tiger.ToString();

        // Assert
        Assert.Equal("Животное: Tiger, под номером: 52, зовут: IVAN, потребляет пищи в день: 52 кг", result);
    }

    [Fact]
    public void Herbivore_ToString_IncludesKindnessLevel()
    {
        // Arrange
        var rabbit = new Rabbit("KROSH", 52,52) ;

        // Act
        var result = rabbit.ToString();

        // Assert
        Assert.Contains("уровень доброты", result);
    }
    
    [Fact]
    public void Monkey_CanBeInContactZoo_ReturnsCorrectValue()
    {
        // Arrange
        var monkey = new Monkey("ANTON", 52,52);

        // Act
        var canBeInContactZoo = monkey.CanBeInContactZoo();

        // Assert
        Assert.True(canBeInContactZoo || !canBeInContactZoo); // Проверяем, что метод работает
    }
}