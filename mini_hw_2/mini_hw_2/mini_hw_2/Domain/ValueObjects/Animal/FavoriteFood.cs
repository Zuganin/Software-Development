namespace mini_hw_2.Domain.Entities;

public class FavoriteFood
{
    Food Food { get; set; }
    public  FavoriteFood(Species species)
    {
        if (species == Species.Herbivores)
        {
            Food = Food.Grass;
        }
        else if (species == Species.Predators)
        {
            Food = Food.Meat;
        }
        else if (species == Species.Birds)
        {
            Food = Food.Insects;
        }
        else if (species == Species.Fish)
        {
            Food = Food.Fishfood;
        }
    }
}