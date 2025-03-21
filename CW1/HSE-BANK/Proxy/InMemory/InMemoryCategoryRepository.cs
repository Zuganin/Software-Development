using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.Repository;

namespace HSE_BANK.Proxy.InMemory;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private Dictionary<Guid, Category> _categories = new();
    public Category GetById(Guid id)
    {
        if (!_categories.ContainsKey(id))
        {
            throw new ArgumentException($"Category with ID {id} not exists");
        }
        return _categories.FirstOrDefault(_category => _category.Key == id).Value;
    }

    public void Add(Category operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        
        if (_categories.ContainsKey(operation.Id))
        {
            throw new ArgumentException($"Category with ID {operation.Id} already exists");
        }
        _categories.Add(operation.Id, operation);
    }
    

    public void Update(Category operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (!_categories.ContainsKey(operation.Id))
        {
            throw new ArgumentException($"Category with ID {operation.Id} not exists");
        }
        _categories[operation.Id] = operation;
    }

    public void Delete(Guid id)
    {
        if (!_categories.ContainsKey(id))
        {
            throw new ArgumentException($"Category with ID {id} not exists");
        }
        _categories.Remove(id);
    }

    public IEnumerable<Category> GetAll()
    {
        return _categories.Values.ToList();
    }

    public IEnumerable<Category> GetByType(CategoryType type)
    {
        return _categories.Where(c =>
            c.Value.Type == type).Select(c => c.Value).ToList();
    }

    public Category GetByName(string name)
    {
        return _categories.First(c => c.Value.Name == name).Value;
    }
}