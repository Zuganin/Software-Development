using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;

namespace HSE_BANK.Interfaces.Repository;

public interface ICategoryRepository : IRepository<Category>
{
    IEnumerable<Category> GetByType(CategoryType type);
    Category GetByName(string name);
}