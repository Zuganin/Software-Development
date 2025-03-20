using HSE_BANK.Interfaces.Export;

namespace HSE_BANK.Domain_Models;

public class BankAccount
{
    internal Guid Id { get; set; }
    internal string Name { get; set; }
    internal decimal Balance { get; set; }
    public BankAccount(string name, decimal balance)
    {
        Id = Guid.NewGuid();
        Name = name;
        Balance = balance;
    }
    
    public void Accept(IExportVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    
};