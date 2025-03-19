using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;

namespace HSE_BANK.Facades;

public interface IOperationFacade
{
    int GetOperationId(Operation operation);
    Operation GetOperationById(int id);
    OperationType getOperationType(Operation operation);
    
    // List<Operation> GetOperationsByAccountId(int accountId);
    // List<Operation> GetOperationsByCategoryId(int categoryId);
    // List<Operation> GetOperationsByDate(DateTime date);
    // List<Operation> GetOperationsByType(OperationType type);
    // List<Operation> GetOperationsByAmount(decimal amount);
    // List<Operation> GetOperationsByDescription(string description);

}