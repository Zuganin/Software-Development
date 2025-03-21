using HSE_BANK.Interfaces.Export;
using CsvHelper.Configuration.Attributes;
namespace HSE_BANK.Domain_Models;

public class BankAccount
{
    [Name("Id")]
    public Guid Id { get; private set; }
    [Name("Name")]
    public string Name { get; private set; }
    [Name("Balance")]
    public decimal Balance { get; private set; }
    public BankAccount(string name, decimal balance)
    {
        Id = Guid.NewGuid();
        Name = name;
        Balance = balance;
    }
    public void UpdateBalance(decimal amount)
    {
        Balance = amount;
    }
    
    public void UpdateName(string name)
    {
        Name = name;
    }
    
    public void Accept(IExportVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    
};