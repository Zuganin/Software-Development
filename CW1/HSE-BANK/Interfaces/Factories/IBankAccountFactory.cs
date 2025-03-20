using HSE_BANK.Domain_Models;

namespace HSE_BANK.Interfaces.IFactories;

public interface IBankAccountFactory
{
    BankAccount CreateBankAccount( string name, decimal balance);
}