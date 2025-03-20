using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.Command;

namespace HSE_BANK.Commands;

public class CreateOperationCommand : ICommands
{
    private readonly IOperationFacade _operationFacade;
    private OperationType _operationType;
    private Guid _bankAccountId;
    private decimal _amount;
    private DateTime _date;
    private string _description;
    private Category _category;
    
    public CreateOperationCommand(IOperationFacade operationFacade)
    {
        _operationFacade = operationFacade;
    }

    public void Create(OperationType operationType, Guid bankAccountId, decimal amount, DateTime date, string description, Category category)
    {
        _operationType = operationType;
        _bankAccountId = bankAccountId;
        _amount = amount;
        _date = date;
        _description = description;
        _category = category;
    }
    
    public void Execute()
    {
        _operationFacade.CreateOperation(_operationType, _bankAccountId, _amount, _description, _date, _category.Name, _category.Type);
    }
    
}