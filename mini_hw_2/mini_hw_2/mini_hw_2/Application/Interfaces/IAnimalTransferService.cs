using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.Animal;
using mini_hw_2.Domain.Entities.Enclosure;

namespace mini_hw_2.Application.Interfaces;

public interface IAnimalTransferService
{
    public Task TransferAnimal(Animal animal, Enclosure NewEnclosure);
}