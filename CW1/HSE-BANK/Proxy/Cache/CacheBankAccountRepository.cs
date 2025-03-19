using HSE_BANK.Domain_Models;
using HSE_BANK.Interfaces.Repository;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace HSE_BANK.Proxy.Cache;

public class CacheBankAccountRepository : IBankAccountRepository 
{
    
    private readonly IBankAccountRepository _realRepository;
    private readonly IMemoryCache _cache;
    private const string CacheAllAccountsKey = "BankAccounts";
    private const string CacheAccountKey = "BankAccount:";
    
    private static readonly TimeSpan CacheLife = TimeSpan.FromMinutes(5);
    
    public CacheBankAccountRepository(IBankAccountRepository realRepository, IMemoryCache cache)
    {
        _realRepository = realRepository;
        _cache = cache;
    }
    
    public BankAccount GetById(Guid id)
    {
        if (_cache.TryGetValue(CacheAccountKey + id, out BankAccount? account) && account != null)
        {
            return account;
        }
        account = _realRepository.GetById(id);
        _cache.Set(CacheAccountKey + id, account, CacheLife);
        return account;
    }

    public void Add(BankAccount category)
    {
        _realRepository.Add(category);
        _cache.Remove(CacheAllAccountsKey);
    }

    public void Update(BankAccount account)
    {
        _realRepository.Update(account);
        _cache.Remove(CacheAllAccountsKey);
    }

    public void Delete(Guid id)
    {
        _realRepository.Delete(id);
        _cache.Remove(CacheAccountKey+ id);
        _cache.Remove(CacheAllAccountsKey);
    }

    public IEnumerable<BankAccount> GetAll()
    {
        if (_cache.TryGetValue(CacheAllAccountsKey , out IEnumerable<BankAccount>? accounts) && accounts != null)
        {
            return accounts;
        }
        accounts = _realRepository.GetAll();
        var bankAccounts = accounts.ToList();
        _cache.Set(CacheAllAccountsKey, accounts, CacheLife);
        return bankAccounts;   
    }

    public BankAccount GetByName(string name)
    {
        if (_cache.TryGetValue(CacheAccountKey + name, out BankAccount? account) && account != null)
        {
            return account;
        }
        account = _realRepository.GetByName(name);
        _cache.Set(CacheAccountKey + name, account, CacheLife);
        return account;
    }
}