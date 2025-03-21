using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Proxy.InMemory;
using Moq;

namespace Tests.Proxy.InMemory;

public class InMemoryOperationRepositoryTests
{
    private readonly InMemoryOperationRepository _repository;
    private readonly Mock<ICategoryFactory> _categoryFactoryMock;

    public InMemoryOperationRepositoryTests()
    {
        _categoryFactoryMock = new Mock<ICategoryFactory>();
        _repository = new InMemoryOperationRepository();
    }

    [Fact]
    public void GetById_ShouldReturnOperation_WhenExists()
    {
        // Arrange
        var operation = new Operation(OperationType.Expenses, Guid.NewGuid(), 100, 
            DateTime.Now, "Тест", "Еда", CategoryType.Supermarkets, _categoryFactoryMock.Object);
        _repository.Add(operation);

        // Act
        var result = _repository.GetById(operation.Id);

        // Assert
        Assert.Equal(operation, result);
    }

    [Fact]
    public void GetById_ShouldThrowException_WhenOperationDoesNotExist()
    {
        // Arrange
        var operationId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _repository.GetById(operationId));
        Assert.Contains("Operation with ID", exception.Message);
    }

    [Fact]
    public void Add_ShouldAddOperation_WhenValid()
    {
        // Arrange
        var operation = new Operation(OperationType.Expenses, Guid.NewGuid(), 200, 
            DateTime.Now, "Такси", "Транспорт", CategoryType.Transport, _categoryFactoryMock.Object);

        // Act
        _repository.Add(operation);
        var result = _repository.GetById(operation.Id);

        // Assert
        Assert.Equal(operation, result);
    }

    [Fact]
    public void Add_ShouldThrowException_WhenOperationIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Add(null));
    }

    [Fact]
    public void Update_ShouldUpdateOperation_WhenExists()
    {
        // Arrange
        var category = new Category("Ресторан", CategoryType.Cafe);
        var operation = new Operation(OperationType.Expenses, Guid.NewGuid(), 500, 
            DateTime.Now, "Обед",category);
        _repository.Add(operation);

        var updateCategory = new Category("Ресторан", CategoryType.Cafe);
        var updatedOperation = operation.Update( OperationType.Expenses, operation.Id, 700, 
            DateTime.Now, "Обед", updateCategory);

        // Act
        _repository.Update(updatedOperation);
        var result = _repository.GetById(operation.Id);

        // Assert
        Assert.Equal(700, result.Amount);
    }

    [Fact]
    public void Delete_ShouldRemoveOperation_WhenExists()
    {
        // Arrange
        var operation = new Operation(OperationType.Expenses, Guid.NewGuid(), 100,
            DateTime.Now, "Супермаркет", "Продукты", CategoryType.Supermarkets, _categoryFactoryMock.Object);
        _repository.Add(operation);

        // Act
        _repository.Delete(operation.Id);

        // Assert
        Assert.Throws<ArgumentException>(() => _repository.GetById(operation.Id));
    }

    [Fact]
    public void GetAll_ShouldReturnAllOperations()
    {
        // Arrange
        var operation1 = new Operation(OperationType.Enrollments, Guid.NewGuid(), 1000,
            DateTime.Now, "Зарплата", "Работа", CategoryType.Salary, _categoryFactoryMock.Object);
        var operation2 = new Operation(OperationType.Expenses, Guid.NewGuid(), 200,
            DateTime.Now, "Кафе", "Еда", CategoryType.Cafe, _categoryFactoryMock.Object);

        _repository.Add(operation1);
        _repository.Add(operation2);

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetOperationsByAccountId_ShouldReturnFilteredData()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var operation1 = new Operation(OperationType.Expenses, accountId, 500,
            DateTime.Now, "Транспорт", "Такси", CategoryType.Transport, _categoryFactoryMock.Object);
        var operation2 = new Operation(OperationType.Enrollments, Guid.NewGuid(), 1000,
            DateTime.Now, "Зарплата", "Работа", CategoryType.Salary, _categoryFactoryMock.Object);

        _repository.Add(operation1);
        _repository.Add(operation2);

        // Act
        var result = _repository.GetOperationsByAccountId(accountId);

        // Assert
        Assert.Single(result);
        Assert.Equal(accountId, result.First().BankAccountId);
    }


    [Fact]
    public void GetOperationsByDate_ShouldReturnFilteredData()
    {
        // Arrange
        var start = DateTime.Now.AddDays(-5);
        var end = DateTime.Now;
        var operation = new Operation(OperationType.Enrollments, Guid.NewGuid(),
            1000, DateTime.Now.AddDays(-2), "Бонус", "Кэшбэк", CategoryType.CashBack, _categoryFactoryMock.Object);
        _repository.Add(operation);

        // Act
        var result = _repository.GetOperationsByDate(start, end);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public void GetOperationsByDateAndAccountId_ShouldReturnFilteredData()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var start = DateTime.Now.AddDays(-7);
        var end = DateTime.Now;
        var operation = new Operation(OperationType.Expenses, accountId, 200,
            DateTime.Now.AddDays(-3), "Транспорт", "Такси", CategoryType.Transport, _categoryFactoryMock.Object);
        _repository.Add(operation);

        // Act
        var result = _repository.GetOperationsByDateAndAccountId(accountId, start, end);

        // Assert
        Assert.Single(result);
        Assert.Equal(accountId, result.First().BankAccountId);
    }
}