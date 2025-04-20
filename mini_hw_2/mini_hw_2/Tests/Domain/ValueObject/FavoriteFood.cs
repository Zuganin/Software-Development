using mini_hw_2.Domain.Entities;

namespace Tests.Domain.ValueObject;

public class FavoriteFoodTests
{
    [Fact]
    public void Constructor_ShouldSetFood_Grass_ForHerbivores()
    {
        var favoriteFood = new FavoriteFood(Species.Herbivores);
        Assert.Equal(Food.Grass, GetFood(favoriteFood));
    }

    [Fact]
    public void Constructor_ShouldSetFood_Meat_ForPredators()
    {
        var favoriteFood = new FavoriteFood(Species.Predators);
        Assert.Equal(Food.Meat, GetFood(favoriteFood));
    }

    [Fact]
    public void Constructor_ShouldSetFood_Insects_ForBirds()
    {
        var favoriteFood = new FavoriteFood(Species.Birds);
        Assert.Equal(Food.Insects, GetFood(favoriteFood));
    }

    [Fact]
    public void Constructor_ShouldSetFood_Fishfood_ForFish()
    {
        var favoriteFood = new FavoriteFood(Species.Fish);
        Assert.Equal(Food.Fishfood, GetFood(favoriteFood));
    }

    // Хак: доступ к private-свойству через рефлексию
    private Food GetFood(FavoriteFood favoriteFood)
    {
        var foodField = typeof(FavoriteFood).GetProperty("Food", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (foodField != null)
        {
            return (Food)foodField.GetValue(favoriteFood);
        }

        var field = typeof(FavoriteFood).GetField("<Food>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (Food)field.GetValue(favoriteFood);
    }
}