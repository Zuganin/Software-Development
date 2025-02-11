using hw1.Models.Animals;
using hw1.Models.Interfaces;

namespace hw1.Services;

public class Vetclinic : IVetclinic
{
    public bool CheckHealth(Animal animal)
    {
        Random random = new Random();
        animal.IsHealthy = random.Next(0, 2) == 1; // 50% шанс, что животное здорово
        return animal.IsHealthy;
    }
}