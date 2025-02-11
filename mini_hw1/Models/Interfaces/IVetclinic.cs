using hw1.Models.Animals;

namespace hw1.Models.Interfaces;


public interface IVetclinic
{
    public bool CheckHealth(Animal animal);
}