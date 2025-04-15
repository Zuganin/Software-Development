using mini_hw_2.Application.Interfaces;

namespace mini_hw_2.Infratructure.Persistence;

public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly Dictionary<Guid, T> _storage = new();

    public Task AddAsync(T entity)
    {
        var entityWithId = entity as dynamic;
        _storage[entityWithId.Id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _storage.Remove(id);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult(_storage.Values.AsEnumerable());
    }

    public Task<T?> GetByIdAsync(Guid id)
    {
        _storage.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(T entity)
    {
        var entityWithId = entity as dynamic;
        _storage[entityWithId.Id] = entity;
        return Task.CompletedTask;
    }
}