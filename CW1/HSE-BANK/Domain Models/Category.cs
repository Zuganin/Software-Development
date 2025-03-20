using CsvHelper.Configuration.Attributes;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.Export;

namespace HSE_BANK.Domain_Models;

public class Category
{
    [Name ("Id")]
    public Guid Id { get; private set; }
    [Name("Name")]
    public string Name { get; private set; }
    [Name("Type")]
    public CategoryType Type { get; private set; }
    
    public Category( string name, CategoryType type)
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