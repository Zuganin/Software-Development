using HSE_BANK.Domain_Models;
using HSE_BANK.Interfaces.IFactories;

namespace HSE_BANK.Factories;

public class BankAccountFactory : IBankAccountFactory
{
    public BankAccount CreateBankAccount(string name, decimal balance)
    {
        if(string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Name must not be empty");
        }
        return new BankAccount(name, balance);
    }
}