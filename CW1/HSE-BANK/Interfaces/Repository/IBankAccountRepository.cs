using HSE_BANK.Domain_Models;

namespace HSE_BANK.Interfaces.Repository;

public interface IBankAccountRepository : IRepository<BankAccount>
{
    BankAccount GetByName(string name);
}