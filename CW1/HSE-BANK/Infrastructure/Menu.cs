using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace HSE_BANK.Infrastructure;

public class Menu
{ 

    private static IAnalysis _analysisFacade;
    private static IBankAccountFacade _bankAccountFacade;
    private static ICategoryFacade _categoryFacade;
    private static IOperationFacade _operationFacade; 
    
    
    public Menu(IAnalysis analysisFacade, IBankAccountFacade bankAccountFacade, ICategoryFacade categoryFacade, IOperationFacade operationFacade)
    {
        _analysisFacade = analysisFacade;
        _bankAccountFacade = bankAccountFacade;
        _categoryFacade = categoryFacade;
        _operationFacade = operationFacade;
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
                    .AddChoices("Работа со счетами", "Категории", "Операции", "Аналитика", "Выход"));

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
                facade.DeleteBankAccount(account);
                AnsiConsole.MarkupLine("[red]Счет удален![/]");
                break;
            case "Список счетов":
                AnsiConsole.Write(new Table()
                    .Title("Счета")
                    .AddColumn("Название"));
                facade.GetAllBankAccounts().ToList().ForEach(account => AnsiConsole.WriteLine(account.Name));
                AnsiConsole.MarkupLine("[gray](Нажмите Enter, чтобы продолжить)[/]");
                Console.ReadLine();
                break;
        }
    }

    private static void ManageCategories()
    {
        var facade = _serviceProvider.GetService<ICategoryFacade>();

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
                    .AddChoices(facade.GetAllCategoryTypes()));
                facade.CreateCategory(name, type);
                AnsiConsole.MarkupLine("[green]Категория добавлена![/]");
                break;
            case "Удалить категорию":
                var categories = facade.GetAllCategories();
                var nameCategoryToRemove = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите категорию для удаления:")
                        .AddChoices(categories.ToList().Select(t => t.Name)));
                var category = facade.GetCategory(nameCategoryToRemove);
                facade.DeleteCategory(category.Id);
                AnsiConsole.MarkupLine("[red]Категория удалена![/]");
                break;
            case "Список категорий":
                AnsiConsole.Write(new Table()
                    .Title("Категории")
                    .AddColumn("Название")
                    .AddColumn("Тип"));
                facade.GetAllCategories().ToList().ForEach(category => AnsiConsole.WriteLine(category.Name));
                AnsiConsole.MarkupLine("[gray](Нажмите Enter, чтобы продолжить)[/]");
                Console.ReadLine();
                break;
        }
    }

    private static void ManageOperations()
    {
        var operationFacade = _serviceProvider.GetService<IOperationFacade>();
        var bankAccountFacade = _serviceProvider.GetService<IBankAccountFacade>();

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
                var accounts = bankAccountFacade.GetAllBankAccounts();
                var accountName = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите счет:")
                        .AddChoices(accounts.Select(t => t.Name)));
                var account = bankAccountFacade.GetBankAccount(accountName);
                var amount = AnsiConsole.Ask<decimal>("Введите сумму операции:");
                var description = AnsiConsole.Ask<string>("Введите описание:");
                var categoryName = AnsiConsole.Ask<string>("Введите название категории:");
                var categoryType = AnsiConsole.Prompt(new SelectionPrompt<CategoryType>()
                    .Title("Выберите тип категории:")
                    .AddChoices(Enum.GetValues<CategoryType>().ToList()));
                operationFacade.CreateOperation(opType, account.Id, amount, description, DateTime.Now,
                     categoryName,  categoryType);
                AnsiConsole.MarkupLine("[green]Операция добавлена![/]");
                break;
            case "Список операций":
                AnsiConsole.Write(new Table()
                    .Title("Операции")
                    .AddColumn("Дата")
                    .AddColumn("Сумма")
                    .AddColumn("Описание"));
                operationFacade.GetAllOperations().ToList().ForEach( op => AnsiConsole.WriteLine($"{op.Date}, {op.Amount}, {op.Description}"));
                AnsiConsole.MarkupLine("[gray](Нажмите Enter, чтобы продолжить)[/]");
                Console.ReadLine();
                break;
        }
    }

    private static void ShowAnalytics()
    {
        

        var accounts = bankAccountFacade.GetAllBankAccounts();
        var accountName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите счет:")
                .AddChoices(accounts.Select(t => t.Name)));
        var account = bankAccountFacade.GetBankAccount(accountName);
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Выберите вид аналитики:[/]")
                .AddChoices("Разница доходов/расходов", "Группировка по категориям", "Назад"));

        switch (choice)
        {
            case "Разница доходов/расходов":
                var periodStart = AnsiConsole.Ask<DateTime>("Введите начало периода (дд-мм-гггг):");
                var periodEnd = AnsiConsole.Ask<DateTime>("Введите конец периода (дд-мм-гггг):");
                var balanceDiff = analysisFacade.GetTotalAmountByDate(account,periodStart, periodEnd);
                AnsiConsole.MarkupLine($"[yellow]Разница: {balanceDiff}[/]");
                break;
            case "Группировка по категориям":
                var categoryType = AnsiConsole.Prompt(new SelectionPrompt<CategoryType>()
                    .Title("Выберите тип категории:")
                    .AddChoices(Enum.GetValues<CategoryType>().ToList()));
                var operations = analysisFacade.GetOperationsByCategoryType(categoryType);
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
}