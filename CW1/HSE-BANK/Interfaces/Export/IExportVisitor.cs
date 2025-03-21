using HSE_BANK.Domain_Models;

namespace HSE_BANK.Interfaces.Export;

public interface IExportVisitor
{
    void Visit(BankAccount account);
    void Visit(Category category);
    void Visit(Operation operation);
    
    void Close();
}