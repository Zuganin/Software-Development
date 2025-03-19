using HSE_BANK.Domain_Models.Enums;

namespace HSE_BANK.Domain_Models;

public class Operation
{
    internal Guid Id { get; private set; }
    internal OperationType Type { get; private set; }
    internal Guid BankAccountId { get; private set; }
    internal decimal Amount { get; private set; }
    internal DateTime Date { get; private set; }
    internal string Description { get; private set; }
    internal CategoryType CategoryId { get; private set; }
    
    internal Operation( OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description, Category category)
    {
        Id = Guid.NewGuid();
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        Description = description;
        CategoryId = category.Type;
    }
    
    
    
}