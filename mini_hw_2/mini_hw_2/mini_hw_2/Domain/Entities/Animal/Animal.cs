namespace mini_hw_2.Domain.Entities;

public class Animal
{
    public Guid Id { get;  }
    public string Name { get; private set; } 
    public Species Species { get; private set; }
    public DateTime BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    
    public FavoriteFood FavoriteFood { get; private set; } // ValueObject
    public EnclousureType EnclosureType { get; private set; } // ValueObject
    
    public HealtStatus HealtStatus { get; private set; } // ValueObject
    public NutritionStatus NutritionStatus { get; private set; } // ValueObject
    public Guid EnclosureId { get; private set; } 
    public DateTime LastFedTime { get; private set; }  // ValueObject

    public Animal(string name, Gender gender, Species species, EnclousureType enclosureType, Guid enclosureId)
    {
        Id = Guid.NewGuid();
        HealtStatus = HealtStatus.Healthy;
        NutritionStatus = NutritionStatus.Eaten;
        Name = name;
        Gender = gender;
        Species = species;
        FavoriteFood = new FavoriteFood(species);
        EnclosureType = enclosureType;
        EnclosureId = enclosureId;
        BirthDate = DateTime.Now;
        LastFedTime = DateTime.Now;
    }

    public void Feed()
    {
        NutritionStatus = NutritionStatus.Eaten;
        LastFedTime = DateTime.Now;
    }

    public void Heal()
    {
        HealtStatus = HealtStatus.Healthy;
    }
    public void MoveTo(Guid newEnclosureId)
    {
        EnclosureId = newEnclosureId;
    }
}
