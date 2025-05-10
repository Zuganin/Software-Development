using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.Repository;

namespace HSE_BANK.Facades;

public class AnalysisFacade : IAnalysis
{
    private readonly IOperationRepository _operationRepository;
 
    public AnalysisFacade(IOperationRepository operationRepository)
    {
        _operationRepository = operationRepository;
    }
    public decimal GetTotalAmountByDate(BankAccount account, DateTime start, DateTime end)
    {
        var operations = _operationRepository.GetOperationsByDateAndAccountId(account.Id, start, end);
        var enrollment = operations.Where(o => o.Type == OperationType.Enrollments).Sum(o => o.Amount);
        var expense = operations.Where(o => o.Type == OperationType.Expenses).Sum(o => o.Amount);
        account.UpdateBalance(enrollment - expense);
        return enrollment - expense;
    }

    public IEnumerable<Operation> GetOperationsByCategoryType(CategoryType type)
    {
        return _operationRepository.GetOperationsByCategoryType(type);
    }
}