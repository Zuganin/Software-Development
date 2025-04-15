using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.Enclosure;

namespace mini_hw_2.Application.Interfaces;

public interface IAnimalTransferService
{
    public void TransferAnimal(Animal animal, Enclosure NewEnclosure);
}