using System.Collections;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;

namespace HSE_BANK.Facades;

public interface IAnalysis
{
    decimal GetTotalAmountByDate(BankAccount account, DateTime start, DateTime end);
    IEnumerable<Operation> GetOperationsByCategoryType(CategoryType type);
    
}