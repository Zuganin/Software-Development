using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Entities.Enclosure;

namespace mini_hw_2.Infratructure.Persistence;

public class InMemoryEnclosureRepository : InMemoryRepository<Enclosure>, IEnclosureRepository;
