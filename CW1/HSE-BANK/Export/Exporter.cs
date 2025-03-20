using HSE_BANK.Interfaces.Export;
using HSE_BANK.Interfaces.Repository;

public static class Exporter
{
    public static void ExportAll(IBankAccountRepository _bankAccountRepository, ICategoryRepository _categoryRepository,
        IOperationRepository _operationRepository,
        IExportVisitor visitor)
    {
        foreach (var account in _bankAccountRepository.GetAll())
            account.Accept(visitor);
            
        foreach (var category in _categoryRepository.GetAll())
            category.Accept(visitor);
            
        foreach (var operation in _operationRepository.GetAll())
            operation.Accept(visitor);
            
        visitor.Close();
    }
}