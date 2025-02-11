

using hw1.Models.Animals;
using hw1.Models.Interfaces;
using hw1.Services;
using Xunit;
using Moq;
using Assert = Xunit.Assert;

namespace Tests;

public class ZooTest
{
    private readonly Mock<IVetclinic> _mockClinic;
    private readonly Zoo _zooTest;

    public ZooTest()
    {
        _mockClinic = new Mock<IVetclinic>();
        _zooTest = new Zoo(_mockClinic.Object);
    }
    
    [Fact]
    public void AddAnimal_HealthyAnimal_AddsToZoo()
    {
        // Arrange
        
        _mockClinic.Setup(clinic => clinic.CheckHealth(It.IsAny<Animal>())).Returns(true); 
        
        var tiger = new Tiger("IVAN", 10,52) ;

        // Act
        _zooTest.AddAnimal(tiger);

        // Assert
        Assert.Contains(tiger, _zooTest.GetAnimals());
        _mockClinic.Verify(clinic => clinic.CheckHealth(tiger), Times.Once); 
    }
    
    [Fact]
    public void AddAnimal_UnhealthyAnimal_DoesNotAddToZoo()
    {
        // Arrange
        _mockClinic.Setup(clinic => clinic.CheckHealth(It.IsAny<Animal>())).Returns(false); 
        var rabbit = new Rabbit("KROSH", 2,1) ;

        // Act
        _zooTest.AddAnimal(rabbit);

        // Assert
        Assert.DoesNotContain(rabbit, _zooTest.GetAnimals());
        _mockClinic.Verify(clinic => clinic.CheckHealth(rabbit), Times.Once); 
    }
    
    
    [Fact]
    public void CalculateTotalFood_ReturnsCorrectSum()
    {
        // Arrange
        var monkey = new Monkey("IVAN", 2, 1);
        var tiger = new Tiger("IVAN", 8, 2);
        
        _mockClinic.Setup(v => v.CheckHealth(It.IsAny<Animal>())).Returns(true);
        
        _zooTest.AddAnimal(monkey);
        _zooTest.AddAnimal(tiger);

        // Act
        var totalFood = _zooTest.CalculateTotalFood();

        // Assert
        Assert.Equal(10.0, totalFood);
    }

}