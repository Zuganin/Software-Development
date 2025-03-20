using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Factories;
using HSE_BANK.Interfaces.Export;
using HSE_BANK.Interfaces.IFactories;

namespace HSE_BANK.Domain_Models;

public class Operation
{
    internal Guid Id { get; private set; }
    internal OperationType Type { get; private set; }
    internal Guid BankAccountId { get; private set; }
    internal decimal Amount { get; private set; }
    internal DateTime Date { get; private set; }
    internal string Description { get; private set; }
    
    internal Category Category { get; private set; }
    
    internal Operation( OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description,
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