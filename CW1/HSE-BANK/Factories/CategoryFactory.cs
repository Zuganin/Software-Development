using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.IFactories;

namespace HSE_BANK.Factories;

public class CategoryFactory : ICategoryFactory
{

    public Category CreateCategory( string name, CategoryType type)
    {
        if ( string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Name must not be empty");
        }
        return new Category( name, type);
    }
}