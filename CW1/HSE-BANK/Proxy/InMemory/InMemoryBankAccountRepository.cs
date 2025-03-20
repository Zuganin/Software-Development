using HSE_BANK.Domain_Models;
using HSE_BANK.Interfaces.Repository;

namespace HSE_BANK.Proxy.InMemory;

public class InMemoryBankAccountRepository : IBankAccountRepository
{
    private readonly Dictionary<Guid, BankAccount> _bankAccounts = new();


    public BankAccount GetByName(string name)
    {
        return _bankAccounts.Values.FirstOrDefault(account => account.Name == name);
    }

    public BankAccount GetById(Guid id)
    {
        return _bankAccounts.Values.FirstOrDefault(account => account.Id == id);
    }

    public void Add(BankAccount category)
    {
        ArgumentNullException.ThrowIfNull(category);
            
        if (_bankAccounts.ContainsKey(category.Id))
        {
            throw new ArgumentException($"Bank account with ID {category.Id} already exists");
        }
        _bankAccounts.Add(category.Id, category);
        
    }

    public void Update(BankAccount account)
    {
        ArgumentNullException.ThrowIfNull(account);

        if (!_bankAccounts.ContainsKey(account.Id))
        {
            throw new ArgumentException($"Bank account with ID {account.Id} not exists");
        }
        _bankAccounts[account.Id] = account;
    }

    public void Delete(Guid id)
    {
        if (!_bankAccounts.ContainsKey(id))
        {
            throw new ArgumentException($"Bank account with ID {id} not exists");
        }
        _bankAccounts.Remove(id);
    }

    public IEnumerable<BankAccount> GetAll()
    {
        return _bankAccounts.Values.ToList();
    }
}