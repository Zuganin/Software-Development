using HSE_BANK.Domain_Models.Enums;

namespace HSE_BANK.Domain_Models;

public class Category
{
    internal Guid Id { get; private set; }
    internal string Name { get; private set; }
    
    internal CategoryType Type { get; private set; }
    
    internal Category( string name, CategoryType type)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
    }
}