using hw1.Models.Animals;
using hw1.Models.Interfaces;
using hw1.Models.Things;


namespace hw1.Services;

public class Zoo 
{
    private IVetclinic _clinic { get;  }
    private List<Thing> _things { get; } = new List<Thing>();
    private List<Animal> _animals { get; } = new List<Animal>();

    public Zoo(IVetclinic clinic)
    {
        _clinic = clinic;
    }
    
    public List<Animal> GetAnimals()
    {
        return _animals;
    }
    
    public List<Thing> GetThings()
    {
        return _things;
    }
    
    public void AddAnimal(Animal animal)
    {
        if (_clinic.CheckHealth(animal))
        {
            _animals.Add(animal);
            Console.WriteLine($"{animal.Name} добавлен в зоопарк");
        }
        else
        {
            Console.WriteLine($"{animal.Name} болеет, ветклиника не можешь позволить добавить его в зоопарк.");
        }
    }

    public void AddThing(Thing thing)
    {
        _things.Add(thing);
        Console.WriteLine($"{thing} добавлен на склад зоопарка.");
    }
    public int CalculateTotalFood() => _animals.Sum(animal => animal.Food);
}