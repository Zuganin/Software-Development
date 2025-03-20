using System.Collections;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace HSE_BANK.Proxy.Cache;

public class CacheOperationRepository : IOperationRepository
{
    private IOperationRepository _realRepository;
    private readonly IMemoryCache _cache;
    private const string CacheAllOperationsKey = "AllOperations";
    private const string CacheOperationKey = "Operation:";
    
    private static readonly TimeSpan CacheLife = TimeSpan.FromMinutes(5);
    public Operation GetById(Guid id)
    {
        if (_cache.TryGetValue(CacheOperationKey + id, out Operation? operation) && operation != null)
        {
            return operation;
        }
        operation = _realRepository.GetById(id);
        _cache.Set(CacheOperationKey + id, operation, CacheLife);
        return operation;
    }

    public void Add(Operation category)
    {
        _realRepository.Add(category);
        _cache.Remove(CacheAllOperationsKey);
    }

    public void Update(Operation odj)
    {
        _realRepository.Update(odj);
        _cache.Remove(CacheAllOperationsKey);
    }

    public void Delete(Guid id)
    {
        _realRepository.Delete(id);
        _cache.Remove(CacheOperationKey+ id);
        _cache.Remove(CacheAllOperationsKey);
    }

    public IEnumerable<Operation> GetAll()
    {
        if (_cache.TryGetValue(CacheAllOperationsKey, out IEnumerable<Operation>? operations) && operations != null)
        {
            return operations;
        }
        operations = _realRepository.GetAll();
        _cache.Set(CacheAllOperationsKey, operations, CacheLife);
        return operations.ToList();
    }
    

    public IEnumerable<Operation> GetOperationsByAccountId(Guid id)
    {
        if (_cache.TryGetValue(CacheOperationKey + id, out IEnumerable<Operation>? operations) && operations != null)
        {
            return operations;
        }
        operations = _realRepository.GetAll();
        _cache.Set(CacheOperationKey+ id, operations, CacheLife);
        return operations.ToList();
    }

    public IEnumerable<Operation> GetOperationsByDate(DateTime start, DateTime end)
    {
        var operations = GetAll();
        return operations.Where(op =>  op.Date >= start && op.Date <= end);
    }

    public IEnumerable<Operation> GetOperationsByDateAndAccountId(Guid accountId, DateTime start, DateTime end)
    {
        var operations = GetAll();
        return operations.Where(op => op.Date >= start && op.Date <= end && op.BankAccountId == accountId);
    }
    
    public IEnumerable<Operation> GetOperationsByCategoryType(CategoryType type)
    {
        var operations = GetAll();
        return operations.Where(op => op.Category.Type == type);
    }

    
}