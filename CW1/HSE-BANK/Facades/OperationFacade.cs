using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Factories;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;

namespace HSE_BANK.Facades;

public class OperationFacade : IOperationFacade
{
    private readonly IOperationFactory _operationFactory;
    private readonly ICategoryFactory _categoryFactory;
    private readonly IOperationRepository _operationRepository;
    private readonly ICategoryRepository _categoryRepository;

    public Operation CreateOperation(OperationType type, BankAccount account, decimal amount, string description, DateTime date,
        string categoryName, CategoryType categoryType)
    {
        Operation operation = _operationFactory.CreateOperation(type, account.Id, amount, date, description, categoryName, categoryType, _categoryFactory);
        _operationRepository.Add(operation);
        _categoryRepository.Add(operation.Category);
        return  operation;
    }
    
    public Operation GetOperationById(Guid id)
    {
        return _operationRepository.GetById(id);
    }
    
    public void UpdateOperation(Operation operation)
    {
        _operationRepository.Update(operation);
    }
    
    
    public void DeleteOperation(Operation operation)
    {
        _operationRepository.Delete(operation.Id);
    }
    
}