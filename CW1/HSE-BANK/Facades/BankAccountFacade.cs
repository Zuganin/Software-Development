using HSE_BANK.Domain_Models;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;

namespace HSE_BANK.Facades;

public class BankAccountFacade : IBankAccountFacade
{
    private readonly IBankAccountFactory _bankAccountFactory;
    private readonly IBankAccountRepository _bankAccountRepository;
    
    public BankAccount CreateBankAccount(string name, decimal  amount = 0)
    {
        var account = _bankAccountFactory.CreateBankAccount(name, amount);
        _bankAccountRepository.Add(account);
        return account;
    }

    public BankAccount GetBankAccount(Guid id)
    {
        return _bankAccountRepository.GetById(id);
    }
    
    public BankAccount GetBankAccount(string name)
    {
        return _bankAccountRepository.GetByName(name);
    }
    public IEnumerable<BankAccount> GetAllBankAccounts()
    {
        return _bankAccountRepository.GetAll();
    }

    public void UpdateBankAccount(BankAccount account)
    {
        _bankAccountRepository.Update(account);
    }

    public void DeleteBankAccount(BankAccount account)
    {
        _bankAccountRepository.Delete(account.Id);
    }
}