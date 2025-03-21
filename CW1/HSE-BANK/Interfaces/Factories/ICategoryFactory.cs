using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;

namespace HSE_BANK.Interfaces.IFactories;

public interface ICategoryFactory
{ 
    Category CreateCategory( string name, CategoryType type);
}