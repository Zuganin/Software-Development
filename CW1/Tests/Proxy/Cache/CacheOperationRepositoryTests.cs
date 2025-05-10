using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Proxy.Cache;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Tests.Proxy.Cache;

public class CacheOperationRepositoryTests
{
    private readonly Mock<IOperationRepository> _realRepositoryMock;
    private readonly IMemoryCache _cache;
    private readonly CacheOperationRepository _cacheOperationRepository;

    public CacheOperationRepositoryTests()
    {
        _realRepositoryMock = new Mock<IOperationRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _cacheOperationRepository = new CacheOperationRepository(_realRepositoryMock.Object, _cache);
    }

    [Fact]
    public void GetById_ShouldReturnFromCache_WhenDataExists()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var expectedOperation = new Operation(OperationType.Expenses, Guid.NewGuid(), 100, 
            DateTime.Now, "Тест", "Еда", CategoryType.Supermarkets, Mock.Of<ICategoryFactory>());
        _cache.Set("Operation:" + operationId, expectedOperation);

        // Act
        var result = _cacheOperationRepository.GetById(operationId);

        // Assert
        Assert.Equal(expectedOperation, result);
        _realRepositoryMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetById_ShouldFetchFromRepository_WhenNotInCache()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var expectedOperation = new Operation(OperationType.Enrollments, Guid.NewGuid(), 5000,
            DateTime.Now, "Зарплата", "Зарплата", CategoryType.Salary, Mock.Of<ICategoryFactory>());
        _realRepositoryMock.Setup(r => r.GetById(operationId)).Returns(expectedOperation);

        // Act
        var result = _cacheOperationRepository.GetById(operationId);

        // Assert
        Assert.Equal(expectedOperation, result);
        _realRepositoryMock.Verify(r => r.GetById(operationId), Times.Once);
    }

    [Fact]
    public void Add_ShouldInvalidateCache()
    {
        // Arrange
        var operation = new Operation(OperationType.Expenses, Guid.NewGuid(), 200,
            DateTime.Now, "Такси", "Транспорт", CategoryType.Transport, Mock.Of<ICategoryFactory>());

        // Act
        _cacheOperationRepository.Add(operation);

        // Assert
        Assert.False(_cache.TryGetValue("AllOperations", out _));
        _realRepositoryMock.Verify(r => r.Add(operation), Times.Once);
    }

    [Fact]
    public void GetAll_ShouldReturnCachedData()
    {
        // Arrange
        var operations = new List<Operation>
        {
            new Operation(OperationType.Enrollments, Guid.NewGuid(), 1000,
                DateTime.Now, "Бонус", "Бонусы", CategoryType.CashBack, Mock.Of<ICategoryFactory>()),
            new Operation(OperationType.Expenses, Guid.NewGuid(), 500,
                DateTime.Now, "Обед", "Кафе", CategoryType.Cafe, Mock.Of<ICategoryFactory>())
        };
        _cache.Set("AllOperations", operations);

        // Act
        var result = _cacheOperationRepository.GetAll();

        // Assert
        Assert.Equal(operations, result);
        _realRepositoryMock.Verify(r => r.GetAll(), Times.Never);
    }

    [Fact]
    public void Delete_ShouldInvalidateCache()
    {
        // Arrange
        var operationId = Guid.NewGuid();

        // Act
        _cacheOperationRepository.Delete(operationId);

        // Assert
        Assert.False(_cache.TryGetValue("Operation:" + operationId, out _));
        _realRepositoryMock.Verify(r => r.Delete(operationId), Times.Once);
    }

    [Fact]
    public void GetOperationsByAccountId_ShouldReturnFilteredData()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var operations = new List<Operation>
        {
            new Operation(OperationType.Enrollments, accountId, 500,
                DateTime.Now, "Пополнение", "Зарплата", CategoryType.Salary, Mock.Of<ICategoryFactory>()),
            new Operation(OperationType.Expenses, Guid.NewGuid(), 100,
                DateTime.Now, "Обед", "Кафе", CategoryType.Cafe, Mock.Of<ICategoryFactory>())
        };
        _realRepositoryMock.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _cacheOperationRepository.GetOperationsByAccountId(accountId);

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
        var operations = new List<Operation>
        {
            new Operation(OperationType.Enrollments, Guid.NewGuid(), 1000,
                DateTime.Now.AddDays(-3), "Бонус", "Бонусы", CategoryType.CashBack, Mock.Of<ICategoryFactory>()),
            new Operation(OperationType.Expenses, Guid.NewGuid(), 500,
                DateTime.Now.AddDays(-10), "Старый платеж", "Развлечения", CategoryType.Entertainment, Mock.Of<ICategoryFactory>())
        };
        _realRepositoryMock.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _cacheOperationRepository.GetOperationsByDate(start, end);

        // Assert
        Assert.Single(result);
        Assert.InRange(result.First().Date, start, end);
    }

    [Fact]
    public void GetOperationsByDateAndAccountId_ShouldReturnFilteredData()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var start = DateTime.Now.AddDays(-7);
        var end = DateTime.Now;
        var operations = new List<Operation>
        {
            new Operation(OperationType.Expenses, accountId, 200, DateTime.Now.AddDays(-3),
                "Транспорт", "Такси", CategoryType.Transport, Mock.Of<ICategoryFactory>()),
            new Operation(OperationType.Enrollments, Guid.NewGuid(), 500,
                DateTime.Now.AddDays(-1), "Зарплата", "Работа", CategoryType.Salary, Mock.Of<ICategoryFactory>())
        };
        _realRepositoryMock.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _cacheOperationRepository.GetOperationsByDateAndAccountId(accountId, start, end);

        // Assert
        Assert.Single(result);
        Assert.Equal(accountId, result.First().BankAccountId);
    }

    [Fact]
    public void GetOperationsByCategoryType_ShouldReturnFilteredData()
    {
        // Arrange
        var categoryType = CategoryType.Entertainment;
        var fitstCategory = new Category("Развлечения", CategoryType.Entertainment);
        var secondCategory = new Category("Бонусы", CategoryType.CashBack);
        var operations = new List<Operation>
        {
            new Operation(OperationType.Expenses, Guid.NewGuid(), 150,
                DateTime.Now, "Кино", fitstCategory),
            new Operation(OperationType.Enrollments, Guid.NewGuid(), 500,
                DateTime.Now, "Бонус",secondCategory)
        };
        _realRepositoryMock.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _cacheOperationRepository.GetOperationsByCategoryType(categoryType);

        // Assert
        Assert.Single(result);
        Assert.Equal(categoryType, result.First().Category.Type);
    }
}