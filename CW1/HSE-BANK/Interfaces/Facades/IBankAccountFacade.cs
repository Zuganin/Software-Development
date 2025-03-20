using HSE_BANK.Domain_Models;

namespace HSE_BANK.Facades;

public interface IBankAccountFacade
{
    BankAccount CreateBankAccount(string name, decimal amount = 0);
    
    BankAccount GetBankAccount(Guid id);
    
    void UpdateBankAccount(BankAccount account);
    
    void DeleteBankAccount(BankAccount account);
    
}