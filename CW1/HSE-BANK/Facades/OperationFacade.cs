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

    public OperationFacade(IOperationFactory operationFactory, ICategoryFactory categoryFactory, IOperationRepository operationRepository, ICategoryRepository categoryRepository)
    {
        _operationFactory = operationFactory;
        _categoryFactory = categoryFactory;
        _operationRepository = operationRepository;
        _categoryRepository = categoryRepository;
    }
    
    public Operation CreateOperation(OperationType type, Guid accountId, decimal amount, string description, DateTime date,
        string categoryName, CategoryType categoryType)
    {
        Operation operation = _operationFactory.CreateOperation(type, accountId, amount, date, description, categoryName, categoryType, _categoryFactory);
        _operationRepository.Add(operation);
        _categoryRepository.Add(operation.Category);
        return  operation;
    }
    
    public IEnumerable<Operation> GetAllOperations()
    {
        return _operationRepository.GetAll();
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