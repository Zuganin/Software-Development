using CsvHelper.Configuration.Attributes;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Factories;
using HSE_BANK.Interfaces.Export;
using HSE_BANK.Interfaces.IFactories;

namespace HSE_BANK.Domain_Models;

public class Operation
{
    [Name ("Id")]
    public Guid Id { get; private set; }
    [Name("Type")]
    public OperationType Type { get; private set; }
    [Name("BankAccountId")]
    public Guid BankAccountId { get; private set; }
    [Name("Amount")]
    public decimal Amount { get; private set; }
    [Name("Date")]
    public DateTime Date { get; private set; }
    [Name("Description")]
    public string Description { get; private set; }
    [Name("Category")]
    public Category Category { get; private set; }
    
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
    
    public void Accept(IExportVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    
}