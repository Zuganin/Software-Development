namespace mini_hw_2.Domain.Entities;

public record SquareMeters
{
    public double Value { get; }

    public SquareMeters(double value)
    {
        if (value <= 0)
            throw new ArgumentException("Размер должен быть положительным");
        Value = value;
    }

    public override string ToString() => $"{Value} м²";
}