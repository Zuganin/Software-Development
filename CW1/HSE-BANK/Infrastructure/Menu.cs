using System.Data.Common;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Export;
using HSE_BANK.Facades;
using HSE_BANK.Import;
using Spectre.Console;

namespace HSE_BANK.Infrastructure;

public class Menu
{ 
    private static IAnalysis _analysisFacade;
    private static IBankAccountFacade _bankAccountFacade;
    private static ICategoryFacade _categoryFacade;
    private static IOperationFacade _operationFacade;
    private static Exporter _exporter;
    
    public Menu(IAnalysis analysisFacade, IBankAccountFacade bankAccountFacade, ICategoryFacade categoryFacade, IOperationFacade operationFacade, Exporter exporter)
    {
        _analysisFacade = analysisFacade;
        _bankAccountFacade = bankAccountFacade;
        _categoryFacade = categoryFacade;
        _operationFacade = operationFacade;
        _exporter = exporter;
    }
    
    public void RunMenu()
    {
        while (true)
        {
            Console.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Выберите действие:[/]")
                    .PageSize(10)
                    .AddChoices("Работа со счетами", "Категории", "Операции", "Аналитика", "Импорт/Экспорт данных", "Выход"));

            switch (choice)
            {
                case "Работа со счетами":
                    ManageBankAccounts();
                    break;
                case "Категории":
                    ManageCategories();
                    break;
                case "Операции":
                    ManageOperations();
                    break;
                case "Аналитика":
                    ShowAnalytics();
                    break;
                case "Импорт/Экспорт данных":
                    ImportExportData();
                    break;
                case "Выход":
                    return;
            }
        }
    }

    private static void ManageBankAccounts()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Выберите действие со счетами:[/]")
                .AddChoices("Добавить счет", "Удалить счет", "Список счетов", "Назад"));

        switch (choice)
        {
            case "Добавить счет":
                var name = AnsiConsole.Ask<string>("Введите название счета:");
                _bankAccountFacade.CreateBankAccount(name);
                AnsiConsole.MarkupLine("[green]Счет добавлен![/]");
                break;
            case "Удалить счет":
                
                var nameAccountToRemove = AnsiConsole.Ask<string>("Введите имя счета для удаления:");
                var account = _bankAccountFacade.GetBankAccount(nameAccountToRemove);
                _bankAccountFacade.DeleteBankAccount(account);
                AnsiConsole.MarkupLine("[red]Счет удален![/]");
                break;
            case "Список счетов":
                AnsiConsole.Write(new Table()
                    .Title("Счета")
                    .AddColumn("Название"));
                _bankAccountFacade.GetAllBankAccounts().ToList().ForEach(ac => AnsiConsole.WriteLine(ac.Name));
                AnsiConsole.MarkupLine("[gray](Нажмите Enter, чтобы продолжить)[/]");
                Console.ReadLine();
                break;
        }
    }

    private static void ManageCategories()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Выберите действие с категориями:[/]")
                .AddChoices("Добавить категорию", "Удалить категорию", "Список категорий", "Назад"));

        switch (choice)
        {
            case "Добавить категорию":
                var name = AnsiConsole.Ask<string>("Введите название категории:");
                var type = AnsiConsole.Prompt(new SelectionPrompt<CategoryType>()
                    .Title("Выберите тип категории:")
                    .AddChoices(_categoryFacade.GetAllCategoryTypes()));
                _categoryFacade.CreateCategory(name, type);
                AnsiConsole.MarkupLine("[green]Категория добавлена![/]");
                break;
            case "Удалить категорию":
                var categories = _categoryFacade.GetAllCategories();
                var nameCategoryToRemove = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите категорию для удаления:")
                        .AddChoices(categories.ToList().Select(t => t.Name)));
                var category = _categoryFacade.GetCategory(nameCategoryToRemove);
                _categoryFacade.DeleteCategory(category.Id);
                AnsiConsole.MarkupLine("[red]Категория удалена![/]");
                break;
            case "Список категорий":
                AnsiConsole.Write(new Table()
                    .Title("Категории")
                    .AddColumn("Название")
                    .AddColumn("Тип"));
                _categoryFacade.GetAllCategories().ToList().ForEach(c => AnsiConsole.WriteLine(c.Name));
                AnsiConsole.MarkupLine("[gray](Нажмите Enter, чтобы продолжить)[/]");
                Console.ReadLine();
                break;
        }
    }

    private static void ManageOperations()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Выберите действие с операциями:[/]")
                .AddChoices("Добавить операцию", "Удалить операцию", "Список операций", "Назад"));

        switch (choice)
        {
            case "Добавить операцию":
                var opType = AnsiConsole.Prompt(new SelectionPrompt<OperationType>()
                    .Title("Выберите тип операции:")
                    .AddChoices(OperationType.Enrollments, OperationType.Expenses));
                var accounts = _bankAccountFacade.GetAllBankAccounts();
                var accountName = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите счет:")
                        .AddChoices(accounts.Select(t => t.Name)));
                var account = _bankAccountFacade.GetBankAccount(accountName);
                var amount = AnsiConsole.Ask<decimal>("Введите сумму операции:");
                var description = AnsiConsole.Ask<string>("Введите описание:");
                var categoryName = AnsiConsole.Ask<string>("Введите название категории:");
                var categoryType = AnsiConsole.Prompt(new SelectionPrompt<CategoryType>()
                    .Title("Выберите тип категории:")
                    .AddChoices(Enum.GetValues<CategoryType>().ToList()));
                _operationFacade.CreateOperation(opType, account.Id, amount, description, DateTime.Now,
                     categoryName,  categoryType);
                AnsiConsole.MarkupLine("[green]Операция добавлена![/]");
                break;
            case "Список операций":
                AnsiConsole.Write(new Table()
                    .Title("Операции")
                    .AddColumn("Дата")
                    .AddColumn("Сумма")
                    .AddColumn("Описание"));
                _operationFacade.GetAllOperations().ToList().ForEach( op => AnsiConsole.WriteLine($"{op.Date}, {op.Amount}, {op.Description}"));
                AnsiConsole.MarkupLine("[gray](Нажмите Enter, чтобы продолжить)[/]");
                Console.ReadLine();
                break;
        }
    }

    private static void ShowAnalytics()
    {
        var accounts = _bankAccountFacade.GetAllBankAccounts();
        var accountName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите счет:")
                .AddChoices(accounts.Select(t => t.Name)));
        var account = _bankAccountFacade.GetBankAccount(accountName);
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Выберите вид аналитики:[/]")
                .AddChoices("Разница доходов/расходов", "Группировка по категориям", "Назад"));

        switch (choice)
        {
            case "Разница доходов/расходов":
                var periodStart = AnsiConsole.Ask<DateTime>("Введите начало периода (гггг-мм-дд):");
                var periodEnd = AnsiConsole.Ask<DateTime>("Введите конец периода (гггг-мм-дд):");
                var balanceDiff = _analysisFacade.GetTotalAmountByDate(account,periodStart, periodEnd);
                AnsiConsole.MarkupLine($"[yellow]Разница: {balanceDiff}[/]");
                break;
            case "Группировка по категориям":
                var categoryType = AnsiConsole.Prompt(new SelectionPrompt<CategoryType>()
                    .Title("Выберите тип категории:")
                    .AddChoices(Enum.GetValues<CategoryType>().ToList()));
                var operations = _analysisFacade.GetOperationsByCategoryType(categoryType);
                var table = new Table().Title($"Группировка доходов/расходов по категории: {categoryType}")
                    .AddColumn($"Номер")
                    .AddColumn("Сумма");
                foreach (var (i, sum, date, description) in operations.Select((op, i) => (i + 1, op.Amount, op.Date, op.Description)))
                {
                    table.AddRow(i.ToString(), sum.ToString("F2"));
                }
                AnsiConsole.Write(table);
                break;
        }
        AnsiConsole.MarkupLine("[gray](Нажмите Enter, чтобы продолжить)[/]");
        Console.ReadLine();
    }
    
    
    private static void ImportExportData()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Выберите действие:[/]")
                .AddChoices("Экспорт данных", "Импорт данных", "Назад"));

        switch (choice)
        {
            case "Экспорт данных":
                ExportData();
                break;
            case "Импорт данных":
                ImportData();
                break;
        }
    }
    

    private static void ExportData()
    {
        string path = AnsiConsole.Ask<string>("Введите путь к файлу:");
        var format = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите формат экспорта:")
                .AddChoices("CSV","JSON", "YAML"));
        
        switch (format)
        {
            case "CSV":
                _exporter.SetVisitor(new CsvExportVisitor(path));
                break;
            case "JSON":
                _exporter.SetVisitor(new JsonExportVisitor(path));
                break;
            case "YAML":
                _exporter.SetVisitor(new YamlExportVisitor(path));
                break;
        }
        Exporter.ExportAll(_bankAccountFacade.GetAllBankAccounts(), _categoryFacade.GetAllCategories(), _operationFacade.GetAllOperations());
        AnsiConsole.MarkupLine($"[green]Данные экспортированы в {path}![/]");
    }

    private static void ImportData()
    {
        string path = AnsiConsole.Ask<string>("Введите путь к файлу:");
        if (!File.Exists(path))
        {
            AnsiConsole.MarkupLine("[red]Ошибка! Файл не найден.[/]");
            return;
        }
        var format = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите формат экспорта:")
                .AddChoices("CSV","JSON", "YAML"));
        
        switch (format)
        {
            case "CSV":
                ImportCsv(path);
                break;
            case "JSON":
                ImportJson(path);
                break;
            case "YAML":
                ImportYaml(path);
                break;
        }
        
        AnsiConsole.MarkupLine("[green]Данные успешно импортированы![/]");
    }

    private static void ImportCsv(string path)
    {
        CsvImporter<BankAccount> csvBankAccountImporter = new();
        var bankAccounts = csvBankAccountImporter.ImportData(path);
        foreach (var account in bankAccounts)
        {
            _bankAccountFacade.CreateBankAccount(account.Name);
        }
        CsvImporter<Category> csvCategoryImporter = new();
        var categories = csvCategoryImporter.ImportData(path);
        foreach (var category in categories)
        {
            _categoryFacade.CreateCategory(category.Name, category.Type);
        }
        CsvImporter<Operation> csvOperationImporter = new();
        var operations = csvOperationImporter.ImportData(path);
        foreach (var operation in operations)
        {
            _operationFacade.CreateOperation(operation.Type, operation.BankAccountId, operation.Amount, operation.Description, operation.Date, operation.Category.Name, operation.Category.Type);
        }
    }

    private static void ImportJson(string path)
    {
        JsonImporter<BankAccount> csvBankAccountImporter = new();
        var bankAccounts = csvBankAccountImporter.ImportData(path);
        foreach (var account in bankAccounts)
        {
            _bankAccountFacade.CreateBankAccount(account.Name);
        }
        JsonImporter<Category> csvCategoryImporter = new();
        var categories = csvCategoryImporter.ImportData(path);
        foreach (var category in categories)
        {
            _categoryFacade.CreateCategory(category.Name, category.Type);
        }
        JsonImporter<Operation> csvOperationImporter = new();
        var operations = csvOperationImporter.ImportData(path);
        foreach (var operation in operations)
        {
            _operationFacade.CreateOperation(operation.Type, operation.BankAccountId, operation.Amount, operation.Description, operation.Date, operation.Category.Name, operation.Category.Type);
        }
    }
    private static void ImportYaml(string path)
    {
        YamlImporter<BankAccount> csvBankAccountImporter = new();
        var bankAccounts = csvBankAccountImporter.ImportData(path);
        foreach (var account in bankAccounts)
        {
            _bankAccountFacade.CreateBankAccount(account.Name);
        }
        YamlImporter<Category> csvCategoryImporter = new();
        var categories = csvCategoryImporter.ImportData(path);
        foreach (var category in categories)
        {
            _categoryFacade.CreateCategory(category.Name, category.Type);
        }
        YamlImporter<Operation> csvOperationImporter = new();
        var operations = csvOperationImporter.ImportData(path);
        foreach (var operation in operations)
        {
            _operationFacade.CreateOperation(operation.Type, operation.BankAccountId, operation.Amount, operation.Description, operation.Date, operation.Category.Name, operation.Category.Type);
        }
    }
    
}