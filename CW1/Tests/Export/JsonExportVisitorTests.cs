using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Export;
using HSE_BANK.Interfaces.Export;
using Newtonsoft.Json;

namespace Tests.Export;

public class JsonExportVisitorTests : IDisposable
{
    private readonly string _outputDir;

    public JsonExportVisitorTests()
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
        IExportVisitor visitor = new JsonExportVisitor(_outputDir);

        // Act
        visitor.Close();

        // Assert
        Assert.False(File.Exists(Path.Combine(_outputDir, "bankAccounts.json")));
        Assert.False(File.Exists(Path.Combine(_outputDir, "categories.json")));
        Assert.False(File.Exists(Path.Combine(_outputDir, "operations.json")));
    }

    [Fact]
    public void Close_ShouldCreateJsonFiles_WithData()
    {
        // Arrange
        IExportVisitor visitor = new JsonExportVisitor(_outputDir);
        var account = new BankAccount("Test Account", 1234);
        var category = new Category("Food", CategoryType.Supermarkets);
        var operation = new Operation(OperationType.Expenses, account.Id, 500, DateTime.Now, "Lunch", 
            category);

        visitor.Visit(account);
        visitor.Visit(category);
        visitor.Visit(operation);

        // Act
        visitor.Close();

        // Assert
        var accountsPath = Path.Combine(_outputDir, "bankAccounts.json");
        var categoriesPath = Path.Combine(_outputDir, "categories.json");
        var operationsPath = Path.Combine(_outputDir, "operations.json");

        Assert.True(File.Exists(accountsPath));
        Assert.True(File.Exists(categoriesPath));
        Assert.True(File.Exists(operationsPath));

        var accountsContent = File.ReadAllText(accountsPath);
        var deserializedAccounts = JsonConvert.DeserializeObject<List<BankAccount>>(accountsContent);
        Assert.NotNull(deserializedAccounts);
        Assert.Single(deserializedAccounts);
        Assert.Equal("Test Account", deserializedAccounts.First().Name);

        var categoriesContent = File.ReadAllText(categoriesPath);
        var deserializedCategories = JsonConvert.DeserializeObject<List<Category>>(categoriesContent);
        Assert.NotNull(deserializedCategories);
        Assert.Single(deserializedCategories);
        Assert.Equal("Food", deserializedCategories.First().Name);

        var operationsContent = File.ReadAllText(operationsPath);

        Assert.Contains("Lunch", operationsContent);
    }

    [Fact]
    public void MultipleVisits_ShouldAccumulateData_InFile()
    {
        // Arrange
        IExportVisitor visitor = new JsonExportVisitor(_outputDir);
        var account1 = new BankAccount("First", 100);
        var account2 = new BankAccount("Second", 200);
        visitor.Visit(account1);
        visitor.Visit(account2);

        // Act
        visitor.Close();

        // Assert
        var accountsPath = Path.Combine(_outputDir, "bankAccounts.json");
        var content = File.ReadAllText(accountsPath);
        Assert.Contains("First", content);
        Assert.Contains("Second", content);
    }

    [Fact]
    public void Close_ShouldOverwriteExistingFile_OnNewVisitorInstance()
    {
        // Arrange
        IExportVisitor visitor1 = new JsonExportVisitor(_outputDir);
        var account1 = new BankAccount("First", 100);
        visitor1.Visit(account1);
        visitor1.Close();
        
        IExportVisitor visitor2 = new JsonExportVisitor(_outputDir);
        var account2 = new BankAccount("Second", 200);
        visitor2.Visit(account2);
        visitor2.Close();

        // Act
        var accountsPath = Path.Combine(_outputDir, "bankAccounts.json");
        var content = File.ReadAllText(accountsPath);

        // Assert
        Assert.Contains("Second", content);
        Assert.DoesNotContain("First", content);
    }
}