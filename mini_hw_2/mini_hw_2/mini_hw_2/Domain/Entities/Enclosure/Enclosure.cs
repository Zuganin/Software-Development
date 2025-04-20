
namespace mini_hw_2.Domain.Entities.Enclosure;

public class Enclosure
{
    public Guid Id { get; init; }
    public EnclosureType Type { get; private set; }
    public SquareMeters Size { get; private set; }
    public int MaxCapacity { get; private set; }

    private readonly List<Animal.Animal> _animals = new();
    public IReadOnlyCollection<Animal.Animal> Animals => _animals.AsReadOnly();

    public Enclosure(Guid id, EnclosureType type, SquareMeters size, int maxCapacity)
    {
        Id = id;
        Type = type;
        Size = size;
        MaxCapacity = maxCapacity;
    }

    public void AddAnimal(Animal.Animal animal)
    {
        if (_animals.Count >= MaxCapacity)
            throw new InvalidOperationException("Вольер переполнен");

        _animals.Add(animal);
    }

    public void RemoveAnimal(Animal.Animal animal)
    {
        if (!_animals.Remove(animal))
            throw new InvalidOperationException("Животное не найдено в вольере");
    }

    public void Clean()
    {
        Console.WriteLine("Уборка вольера проведена");
    }

    public int CurrentAnimalCount => _animals.Count;
}