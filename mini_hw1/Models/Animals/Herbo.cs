using hw1.Models.Interfaces;

namespace hw1.Models.Animals;

public class Herbo : Animal
{
    public int Kindness { get; }
    
    protected Herbo(string name, int food, int number) : base(name , food, number)
    {
        Random random = new Random();
        Kindness = random.Next(1,11);
    }
    public bool CanBeInContactZoo() => Kindness > 5;
    
}
