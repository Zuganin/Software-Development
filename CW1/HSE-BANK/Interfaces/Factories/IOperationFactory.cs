using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;

namespace HSE_BANK.Interfaces.IFactories;

internal interface IOperationFactory
{
    internal Operation CreateOperation( OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description, Category category);
}