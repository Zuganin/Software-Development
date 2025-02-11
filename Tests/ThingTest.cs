using hw1.Models.Things;

namespace Tests;

public class ThingTest
{
    [Fact]
    public void Thing_Properties_AreSetCorrectly()
    {
        // Arrange
        var number = 101;

        // Act
        var thing = new Table(number);

        // Assert
        Assert.Equal(number, thing.Number);
    }
    [Fact]
    public void Computer_Properties_AreSetCorrectly()
    {
        // Arrange
        var number = 102;

        // Act
        var computer = new Computer( number);

        // Assert
        Assert.Equal(number, computer.Number);
    }
}