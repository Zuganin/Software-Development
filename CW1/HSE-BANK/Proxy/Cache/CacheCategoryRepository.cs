using System.Collections;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace HSE_BANK.Proxy.Cache;

public class CacheCategoryRepository : ICategoryRepository
{
    private ICategoryRepository _realRepository;
    private readonly IMemoryCache _cache;
    private const string CacheAllCategoriesKey = "AllCategories";
    private const string CacheCategoryKey = "Category:";
    
    private static readonly TimeSpan CacheLife = TimeSpan.FromMinutes(5);
    
    public CacheCategoryRepository(ICategoryRepository realRepository, IMemoryCache cache)
    {
        _realRepository = realRepository;
        _cache = cache;
    }
    public Category GetById(Guid id)
    {
        if (_cache.TryGetValue(CacheCategoryKey + id, out Category? category) && category != null)
        {
            return category;
        }
        category = _realRepository.GetById(id);
        _cache.Set(CacheCategoryKey + id, category, CacheLife);
        return category;
    }

    public void Add(Category operation)
    {
        _realRepository.Add(operation);
        _cache.Remove(CacheAllCategoriesKey);
    }

    public void Update(Category operation)
    {
        _realRepository.Update(operation);
        _cache.Remove(CacheAllCategoriesKey);
    }

    public void Delete(Guid id)
    {
        _realRepository.Delete(id);
        _cache.Remove(CacheCategoryKey+ id);
        _cache.Remove(CacheAllCategoriesKey);
    }

    public IEnumerable<Category> GetAll()
    {
        if(_cache.TryGetValue(CacheAllCategoriesKey, out IEnumerable<Category>? categories) && categories != null)
        {
            return categories.ToList();
        }
        categories = _realRepository.GetAll();
        _cache.Set(CacheAllCategoriesKey, categories, CacheLife);
        var allCategories = categories.ToList();
        return allCategories;
    }

    public IEnumerable<Category> GetByType(CategoryType type)
    {
        var categories = GetAll();
        return categories.Where(c => c.Type == type);
    }

    public Category GetByName(string name)
    {
        var categories = GetAll();
        return categories.First(c => c.Name == name);
    }
}