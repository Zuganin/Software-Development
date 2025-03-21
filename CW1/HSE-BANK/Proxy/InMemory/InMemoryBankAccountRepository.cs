using HSE_BANK.Domain_Models;
using HSE_BANK.Interfaces.Repository;

namespace HSE_BANK.Proxy.InMemory;

public class InMemoryBankAccountRepository : IBankAccountRepository
{
    private readonly Dictionary<Guid, BankAccount> _bankAccounts = new();


    public BankAccount GetByName(string name)
    {
        if(!_bankAccounts.Values.Any(x => x.Name == name))
        {
            throw new ArgumentException($"Bank account with name {name} not exists");
        }
        return _bankAccounts.Values.FirstOrDefault(x => x.Name == name);
    }

    public BankAccount GetById(Guid id)
    {
        if (!_bankAccounts.ContainsKey(id))
        {
            throw new ArgumentException($"Bank account with ID {id} not exists");
        }
        return _bankAccounts.ContainsKey(id) ? _bankAccounts[id] : null;
    }

    public void Add(BankAccount operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
            
        if (_bankAccounts.ContainsKey(operation.Id))
        {
            throw new ArgumentException($"Bank account with ID {operation.Id} already exists");
        }
        _bankAccounts.Add(operation.Id, operation);
        
    }

    public void Update(BankAccount operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (!_bankAccounts.ContainsKey(operation.Id))
        {
            throw new ArgumentException($"Bank account with ID {operation.Id} not exists");
        }
        _bankAccounts[operation.Id] = operation;
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