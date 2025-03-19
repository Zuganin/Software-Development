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

    public void Add(Category category)
    {
        _realRepository.Add(category);
        _cache.Remove(CacheAllCategoriesKey);
    }

    public void Update(Category odj)
    {
        _realRepository.Update(odj);
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
            return categories;
        }
        categories = _realRepository.GetAll();
        _cache.Set(CacheAllCategoriesKey, categories, CacheLife);
        var allCategories = categories.ToList();
        return allCategories;
    }

    public Category GetByType(CategoryType type)
    {
        if (_cache.TryGetValue(CacheCategoryKey + type, out Category? category) && category != null)
        {
            return category;
        }
        category = _realRepository.GetByType(type);
        _cache.Set(CacheCategoryKey + type, category, CacheLife);
        return category;
    }

    public Category GetByName(string name)
    {
        if (_cache.TryGetValue(CacheCategoryKey + name, out Category? category) && category != null)
        {
            return category;
        }
        category = _realRepository.GetByName(name);
        _cache.Set(CacheCategoryKey + name, category, CacheLife);
        return category;
    }
}