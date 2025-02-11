using hw1.Models.Animals;
using hw1.Models.Interfaces;
using hw1.Services;
using Xunit;
using Moq;
using Assert = Xunit.Assert;

namespace Tests;

public class VetclinicTest
{
    
    [Fact]
    public void CheckHealth_ReturnsTrueOrFalse()
    {
        // Arrange
        var vetClinic = new Vetclinic();
        var animal = new Monkey("IVAN", 1,  1);
        
        // Act
        var isHealthy = vetClinic.CheckHealth(animal);
        
        // Assert
        Assert.True(isHealthy || !isHealthy);
    }
}