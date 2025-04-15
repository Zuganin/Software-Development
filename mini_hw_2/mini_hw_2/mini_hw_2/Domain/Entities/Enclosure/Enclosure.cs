
namespace mini_hw_2.Domain.Entities.Enclosure;

public class Enclosure
{
    public Guid Id { get; set; }
    public EnclousureType Type { get; set; }
    public int Size { get; set; }
    public int Count { get; set; }
    public int MaxCapacity { get; set; }
    
    public void AddAnimal()
    {
        if (Count < MaxCapacity)
        {
            Count++;
        }
        else
        {
            throw new InvalidOperationException("Enclosure is full");
        }
    }
    public void RemoveAnimal()
    {
        if (Count > 0)
        {
            Count--;
        }
        else
        {
            throw new InvalidOperationException("Enclosure is empty");
        }
    }
    
    public void CleanEnclosure()
    {
        // Logic to clean the enclosure
    }
    
}