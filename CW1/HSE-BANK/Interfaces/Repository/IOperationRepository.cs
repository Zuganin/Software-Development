using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;

namespace HSE_BANK.Interfaces.Repository;

public interface IOperationRepository : IRepository<Operation>
{
    IEnumerable<Operation> GetOperationsByAccountId(Guid id);
    IEnumerable<Operation> GetOperationsByDate(DateTime start, DateTime end);
    IEnumerable<Operation> GetOperationsByDateAndAccountId(Guid id, DateTime start, DateTime end);
    IEnumerable<Operation> GetOperationsByCategoryType(CategoryType type);
    
}