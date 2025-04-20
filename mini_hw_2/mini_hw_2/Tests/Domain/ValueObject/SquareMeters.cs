using mini_hw_2.Domain.Entities;

namespace Tests.Domain.ValueObject;

public class SquareMetersTests
{
    [Fact]
    public void Constructor_ShouldSetValue_WhenPositive()
    {
        var square = new SquareMeters(10.5);
        Assert.Equal(10.5, square.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_ShouldThrowArgumentException_WhenNonPositive(double invalidValue)
    {
        Assert.Throws<ArgumentException>(() => new SquareMeters(invalidValue));
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var square = new SquareMeters(42);
        Assert.Equal("42 м²", square.ToString());
    }
}