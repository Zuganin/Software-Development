using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;

namespace HSE_BANK.Factories;

public class Analysis : IAnalysis
{
    private readonly I;
    public decimal GetTotalAmountByDate(BankAccount account, DateTime start, DateTime end)
    {
        return _analysisImplementation.GetTotalAmountByDate(account, start, end);
    }

    public IEnumerable<Operation> GetOperationsByCategoryType(CategoryType type)
    {
        return _analysisImplementation.GetOperationsByCategoryType(type);
    }
}