using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.IFactories;

namespace HSE_BANK.Factories;

public class OperationFactory : IOperationFactory
{
    public Operation CreateOperation( OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description, Category category)
    {
        if( amount <=0  )
        {
            throw new ArgumentException("Amount must be greater than 0");
        }
        if( string.IsNullOrEmpty(description))
        {
            description = "Нет описания операции";  
        }
        return new Operation( type, bankAccountId, amount, date, description, category);
    }
}