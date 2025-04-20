using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.Animal;

namespace mini_hw_2.Infratructure.Persistence;

public class InMemoryAnimalRepository : InMemoryRepository<Animal> , IAnimalRepository;