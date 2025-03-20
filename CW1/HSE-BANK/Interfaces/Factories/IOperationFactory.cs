using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Factories;

namespace HSE_BANK.Interfaces.IFactories;

public interface IOperationFactory
{
    Operation CreateOperation( OperationType type, Guid bankAccountId, decimal amount, DateTime date,
        string description, string categoryName, CategoryType categoryType, ICategoryFactory factory);
}