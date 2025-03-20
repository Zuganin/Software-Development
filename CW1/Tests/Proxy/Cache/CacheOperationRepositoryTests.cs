using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using HSE_BANK.Proxy.Cache;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using System;
using System.Collections.Generic;
using HSE_BANK.Interfaces.IFactories;

public class CacheOperationRepositoryTests
{
    [Fact]
    public void GetById_ShouldReturnFromCache_WhenExists()
    {
        // Arrange
        var repoMock = new Mock<IOperationRepository>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var repository = new CacheOperationRepository(repoMock.Object, cache);

        var operation = new Operation(OperationType.Enrollments, Guid.NewGuid(), 5000, DateTime.Now, "Пополнение", "Зарплата", CategoryType.Salary, Mock.Of<ICategoryFactory>());
        cache.Set("Operation:" + operation.Id, operation);

        // Act
        var result = repository.GetById(operation.Id);

        // Assert
        Assert.Equal(operation, result);
        repoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetOperationsByDate_ShouldReturnCorrectOperations()
    {
        // Arrange
        var repoMock = new Mock<IOperationRepository>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var repository = new CacheOperationRepository(repoMock.Object, cache);

        var operations = new List<Operation>
        {
            new Operation(OperationType.Enrollments, Guid.NewGuid(), 5000, DateTime.Now.AddDays(-2), "Пополнение", "Зарплата", CategoryType.Salary, Mock.Of<ICategoryFactory>()),
            new Operation(OperationType.Expenses, Guid.NewGuid(), 2000, DateTime.Now.AddDays(-1), "Покупка", "Продукты", CategoryType.Supermarkets, Mock.Of<ICategoryFactory>())
        };
        repoMock.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = repository.GetOperationsByDate(DateTime.Now.AddDays(-3), DateTime.Now);

        // Assert
        Assert.Equal(2, result.Count());
    }
}