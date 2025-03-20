using System.Collections;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;

namespace HSE_BANK.Facades;

public interface ICategoryFacade
{
    Category CreateCategory(string name, CategoryType type);
    
    Category GetCategory(Guid id);
    
    Category GetCategory(string name);
    
    IEnumerable<Category> GetAllCategories();

    IEnumerable<CategoryType> GetAllCategoryTypes();
    void UpdateCategory(Category category);
    
    void DeleteCategory(Guid id);
}