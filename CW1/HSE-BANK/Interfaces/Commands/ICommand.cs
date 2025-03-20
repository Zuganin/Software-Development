namespace HSE_BANK.Interfaces.Command;

public interface ICommand
{
    void CreateNewAccount();
    void CreateNewCategory();
    void CreateNewOperation();
    void ImportData();
    void ExportData();
    void ShowAllAccounts();
    void ShowAllCategories();
    void ShowAccountOperations();
    void ShowBalance();
    
}