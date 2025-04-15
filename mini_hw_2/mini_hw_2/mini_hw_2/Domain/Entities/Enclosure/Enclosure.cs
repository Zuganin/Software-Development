
namespace mini_hw_2.Domain.Entities.Enclosure;

public class Enclosure
{
    public Guid Id { get; init; }
    public EnclosureType Type { get; private set; }
    public SquareMeters Size { get; private set; }
    public int MaxCapacity { get; private set; }

    private readonly List<Guid> _animalIds = new();
    public IReadOnlyCollection<Guid> AnimalIds => _animalIds.AsReadOnly();

    public Enclosure(Guid id, EnclosureType type, SquareMeters size, int maxCapacity)
    {
        Id = id;
        Type = type;
        Size = size;
        MaxCapacity = maxCapacity;
    }

    public void AddAnimal(Guid animalId)
    {
        if (_animalIds.Count >= MaxCapacity)
            throw new InvalidOperationException("Вольер переполнен");

        _animalIds.Add(animalId);
    }

    public void RemoveAnimal(Guid animalId)
    {
        if (!_animalIds.Remove(animalId))
            throw new InvalidOperationException("Животное не найдено в вольере");
    }

    public void Clean()
    {
        if (_animalIds.Count == 0)
            throw new InvalidOperationException("Вольер пустой, уборка не требуется");
        Console.WriteLine("Уборка вольера проведена");
    }

    public int CurrentAnimalCount => _animalIds.Count;
}