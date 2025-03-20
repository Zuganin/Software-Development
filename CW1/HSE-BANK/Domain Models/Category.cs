using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.Export;

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
    public void Accept(IExportVisitor visitor)
    {
        visitor.Visit(this);
    }
}