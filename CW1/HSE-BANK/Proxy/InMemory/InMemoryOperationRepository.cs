using System.Collections;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.Repository;

namespace HSE_BANK.Proxy.InMemory;

public class InMemoryOperationRepository : IOperationRepository
{
    private Dictionary<Guid, Operation> _operations = new();

    public Operation GetById(Guid id)
    {
        return _operations[id];
    }

    public void Add(Operation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        
        if (_operations.ContainsKey(operation.Id))
        {
            throw new ArgumentException($"Category with ID {operation.Id} already exists");
        }
        _operations.Add(operation.Id, operation);
    }

    public void Update(Operation obj)
    {
         ArgumentNullException.ThrowIfNull(obj);

         if (!_operations.ContainsKey(obj.Id))
         {
             throw new ArgumentException($"Category with ID {obj.Id} not exists");
         }
         _operations[obj.Id] = obj;
    }

    public void Delete(Guid id)
    {
        if (!_operations.ContainsKey(id))
        {
            throw new ArgumentException($"Category with ID {id} not exists");
        }
        _operations.Remove(id);
    }

    public IEnumerable<Operation> GetAll()
    {
        return _operations.Values.ToList();
    }
    

    public IEnumerable<Operation> GetOperationsByAccountId(Guid id)
    {
        var operations = GetAll();
        return operations.Where(operation => operation.BankAccountId == id);
    }

    public IEnumerable<Operation> GetOperationsByCategoryType(CategoryType type)
    {
        var operations = GetAll();
        return operations.Where(op => op.Category.Type == type);
    }
    
    public IEnumerable<Operation> GetOperationsByDate(DateTime start, DateTime end)
    {
        var operations = GetAll();
        return operations.Where(operation => operation.Date >= start && operation.Date <= end);
    }
    public IEnumerable<Operation> GetOperationsByDateAndAccountId(Guid id, DateTime start, DateTime end)
    {
        var operations = GetAll();
        return operations.Where(operation => operation.Date >= start && operation.Date <= end &&
                                             operation.BankAccountId == id);
    }
}