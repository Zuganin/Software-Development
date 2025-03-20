using Xunit;
using HSE_BANK.Proxy.InMemory;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using System;
using System.Collections.Generic;
using HSE_BANK.Factories;
using HSE_BANK.Interfaces.IFactories;
using Moq;

public class InMemoryOperationRepositoryTests
{
    [Fact]
    public void Add_ShouldStoreOperation()
    {
        // Arrange
        var repository = new InMemoryOperationRepository();
        var factory = new CategoryFactory();
        var operation = new Operation(OperationType.Enrollments, Guid.NewGuid(), 5000, DateTime.Now, "Зарплата", "Зарплата", CategoryType.Salary, factory);

        // Act
        repository.Add(operation);
        var result = repository.GetById(operation.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5000, result.Amount);
    }

    [Fact]
    public void GetOperationsByAccountId_ShouldReturnCorrectOperations()
    {
        // Arrange
        var repository = new InMemoryOperationRepository();
        var accountId = Guid.NewGuid();
        repository.Add(new Operation(OperationType.Enrollments, accountId, 5000, DateTime.Now, "Пополнение", "Зарплата", CategoryType.Salary, Mock.Of<ICategoryFactory>()));
        repository.Add(new Operation(OperationType.Expenses, accountId, 2000, DateTime.Now, "Покупка", "Продукты", CategoryType.Supermarkets, Mock.Of<ICategoryFactory>()));

        // Act
        var result = repository.GetOperationsByAccountId(accountId);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetOperationsByDate_ShouldReturnCorrectOperations()
    {
        // Arrange
        var repository = new InMemoryOperationRepository();
        var now = DateTime.Now;
        repository.Add(new Operation(OperationType.Enrollments, Guid.NewGuid(), 5000, now.AddDays(-2), "Пополнение", "Зарплата", CategoryType.Salary, Mock.Of<ICategoryFactory>()));
        repository.Add(new Operation(OperationType.Expenses, Guid.NewGuid(), 2000, now.AddDays(-1), "Покупка", "Продукты", CategoryType.Supermarkets, Mock.Of<ICategoryFactory>()));

        // Act
        var result = repository.GetOperationsByDate(now.AddDays(-3), now);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void Delete_ShouldRemoveOperation()
    {
        // Arrange
        var repository = new InMemoryOperationRepository();
        var operation = new Operation(OperationType.Expenses, Guid.NewGuid(), 2000, DateTime.Now, "Тест", "Тест", CategoryType.Supermarkets, Mock.Of<ICategoryFactory>());
        repository.Add(operation);

        // Act
        repository.Delete(operation.Id);

        // Assert
        Assert.Throws<ArgumentException>(() => repository.GetById(operation.Id));
    }
}
