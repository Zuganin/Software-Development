using CsvHelper.Configuration.Attributes;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Factories;
using HSE_BANK.Interfaces.Export;
using HSE_BANK.Interfaces.IFactories;

namespace HSE_BANK.Domain_Models;

public class Operation
{
    [Name ("Id")]
    public Guid Id { get;  set; }
    [Name("Type")]
    public OperationType Type { get;  set; }
    [Name("BankAccountId")]
    public Guid BankAccountId { get;  set; }
    [Name("Amount")]
    public decimal Amount { get;  set; }
    [Name("Date")]
    public DateTime Date { get;  set; }
    [Name("Description")]
    public string Description { get;  set; }
    [Name("Category")]
    public Category Category { get;  set; }
    
    public Operation() {}
    
    public Operation( OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description,
         string categoryName, CategoryType categoryType, ICategoryFactory factory)
    {
        Id = Guid.NewGuid();
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        Description = description;
        Category = factory.CreateCategory(categoryName, categoryType);
    }
    
    public Operation (OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description, Category category)
    {
        Id = Guid.NewGuid();
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        Description = description;
        Category = category;
    }
    public Operation Update(OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description, Category category)
    {
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        Description = description;
        Category = category;
        return this;
    }
    
    public void Accept(IExportVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    
}