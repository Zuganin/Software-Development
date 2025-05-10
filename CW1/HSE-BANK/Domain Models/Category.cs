using CsvHelper.Configuration.Attributes;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.Export;

namespace HSE_BANK.Domain_Models;

public class Category
{
    [Name ("Id")]
    public Guid Id { get;  set; }
    [Name("Name")]
    public string Name { get;  set; }
    [Name("Type")]
    public CategoryType Type { get;  set; }
    
    public Category() {}
    
    public Category( string name, CategoryType type)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
    }
    public Category Update(string name, CategoryType type)
    {
        Name = name;
        Type = type;
        return this;
    }
    public void Accept(IExportVisitor visitor)
    {
        visitor.Visit(this);
    }
}