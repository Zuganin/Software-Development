using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;

namespace HSE_BANK.Facades;

public class CategoryFacade : ICategoryFacade
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryFactory _categoryFactory;
    public Category CreateCategory(string name, CategoryType type)
    {
        var category = _categoryFactory.CreateCategory(name, type);
        _categoryRepository.Add(category);
        return category;
    }

    public Category GetCategory(Guid id)
    {
        return _categoryRepository.GetById(id);
    }

    public void UpdateCategory(Category category)
    {
        _categoryRepository.Update(category);
    }

    public void DeleteCategory(Guid id)
    {
        _categoryRepository.Delete(id);
    }
}