using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Entities;

namespace mini_hw_2.Infratructure.Persistence;

public class InMemoryAnimalRepository : InMemoryRepository<Animal> , IAnimalRepository;