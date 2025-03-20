using HSE_BANK.Domain_Models;
using HSE_BANK.Interfaces.Export;
using HSE_BANK.Interfaces.Repository;

public class Exporter
{
    private static IExportVisitor _visitor;
    
    public void SetVisitor(IExportVisitor visitor)
    {
        _visitor = visitor;
    }
    public static void ExportAll(IEnumerable<BankAccount> bankAccounts, IEnumerable<Category> categories,
        IEnumerable<Operation> operations)
    {
        foreach (var account in bankAccounts)
            account.Accept(_visitor);
            
        foreach (var category in categories)
            category.Accept(_visitor);
            
        foreach (var operation in operations)
            operation.Accept(_visitor);
            
        _visitor.Close();
    }
}