using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Export;
using HSE_BANK.Interfaces.Export;

namespace Tests.Export;

public class CsvExportVisitorTests : IDisposable
{
    private readonly string _outputDir;

    public CsvExportVisitorTests()
    {
        _outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_outputDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_outputDir))
            Directory.Delete(_outputDir, true);
    }

    [Fact]
    public void Close_ShouldNotCreateFiles_WhenNoDataVisited()
    {
        // Arrange
        IExportVisitor visitor = new CsvExportVisitor(_outputDir);

        // Act
        visitor.Close();

        // Assert
        Assert.False(File.Exists(Path.Combine(_outputDir, "bankAccounts.csv")));
        Assert.False(File.Exists(Path.Combine(_outputDir, "categories.csv")));
        Assert.False(File.Exists(Path.Combine(_outputDir, "operations.csv")));
    }

    [Fact]
    public void Close_ShouldCreateCsvFiles_WithData()
    {
        // Arrange
        IExportVisitor visitor = new CsvExportVisitor(_outputDir);

        var account = new BankAccount("Основной", 1000);
        var category = new Category("Еда", CategoryType.Supermarkets);
        var operation = new Operation(OperationType.Expenses, account.Id, 500, DateTime.Now, "Покупка", 
            category);

        visitor.Visit(account);
        visitor.Visit(category);
        visitor.Visit(operation);

        // Act
        visitor.Close();

        // Assert: файлы существуют и содержат ожидаемые данные
        var accountsPath = Path.Combine(_outputDir, "bankAccounts.csv");
        var categoriesPath = Path.Combine(_outputDir, "categories.csv");
        var operationsPath = Path.Combine(_outputDir, "operations.csv");

        Assert.True(File.Exists(accountsPath));
        Assert.True(File.Exists(categoriesPath));
        Assert.True(File.Exists(operationsPath));

        var accountsContent = File.ReadAllText(accountsPath);
        var categoriesContent = File.ReadAllText(categoriesPath);
        var operationsContent = File.ReadAllText(operationsPath);

        Assert.Contains("Основной", accountsContent);
        Assert.Contains("1000", accountsContent);

        Assert.Contains("Еда", categoriesContent);

        Assert.Contains("Покупка", operationsContent);
    }

    [Fact]
    public void MultipleVisits_ShouldAccumulateData_InFile()
    {
        // Arrange
        IExportVisitor visitor = new CsvExportVisitor(_outputDir);

        var account1 = new BankAccount("Первый", 500);
        var account2 = new BankAccount("Второй", 2000);
        visitor.Visit(account1);
        visitor.Visit(account2);

        // Act
        visitor.Close();

        // Assert
        var accountsPath = Path.Combine(_outputDir, "bankAccounts.csv");
        var content = File.ReadAllText(accountsPath);
        Assert.Contains("Первый", content);
        Assert.Contains("Второй", content);
    }

    [Fact]
    public void Close_ShouldOverwriteExistingFile_OnNewVisitorInstance()
    {
        // Arrange
        IExportVisitor visitor1 = new CsvExportVisitor(_outputDir);
        var account1 = new BankAccount("Первый", 500);
        visitor1.Visit(account1);
        visitor1.Close();

        IExportVisitor visitor2 = new CsvExportVisitor(_outputDir);
        var account2 = new BankAccount("Второй", 2000);
        visitor2.Visit(account2);
        visitor2.Close();

        // Act
        var accountsPath = Path.Combine(_outputDir, "bankAccounts.csv");
        var content = File.ReadAllText(accountsPath);

        // Assert
        Assert.Contains("Второй", content);
        Assert.DoesNotContain("Первый", content);
    }
}