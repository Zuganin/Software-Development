using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Factories;

namespace HSE_BANK.Interfaces.IFactories;

internal interface IOperationFactory
{
    internal Operation CreateOperation( OperationType type, Guid bankAccountId, decimal amount, DateTime date,
        string description, string categoryName, CategoryType categoryType, ICategoryFactory factory);
}