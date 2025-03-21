using System.Windows.Input;
using HSE_BANK.Interfaces.Command;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.Repository;

namespace HSE_BANK.Commands;

public class CreateBankAccountCommand : ICommands
{
    private readonly IBankAccountFacade _bankAccountFacade;
    private string _name;
    private decimal _balance;

    public CreateBankAccountCommand(IBankAccountFacade bankAccountFacade)
    {
        _bankAccountFacade = bankAccountFacade;
    }

    public void Create(string name, decimal balance)
    {
        _name = name;
        _balance = balance;
    }
    
    public void Execute()
    {
        _bankAccountFacade.CreateBankAccount(_name, _balance);
    }
    
}