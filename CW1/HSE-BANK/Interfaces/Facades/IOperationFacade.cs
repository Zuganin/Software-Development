using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Factories;

namespace HSE_BANK.Facades;

public interface IOperationFacade
{
    Operation CreateOperation(OperationType type, Guid accountId, decimal amount, string description, DateTime date,
        string categoryName, CategoryType categoryType);

    Operation GetOperationById(Guid id);
    
    void UpdateOperation(Operation operation);
    
    void DeleteOperation(Operation operation);


}