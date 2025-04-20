namespace mini_hw_2.Domain.Entities;

public class FavoriteFood
{
    Food Food { get; set; }
    public  FavoriteFood(Species species)
    {
        Food = species switch
        {
            Species.Herbivores => Food.Grass,
            Species.Predators => Food.Meat,
            Species.Birds     => Food.Insects,
            Species.Fish      => Food.Fishfood,
            _ => throw new ArgumentOutOfRangeException(nameof(species), "Неизвестный вид животного")
        };
    }
}