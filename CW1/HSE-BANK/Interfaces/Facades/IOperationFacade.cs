using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Factories;

namespace HSE_BANK.Facades;

public interface IOperationFacade
{
    Operation CreateOperation(OperationType type, Guid accountId, decimal amount, DateTime date,string description,
        string categoryName, CategoryType categoryType);

    IEnumerable<Operation> GetAllOperations();
    
    Operation GetOperationById(Guid id);
    
    void UpdateOperation(Operation operation);
    
    void DeleteOperation(Operation operation);


}