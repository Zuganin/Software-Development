using Xunit;
using Moq;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using System;
using System.Collections.Generic;
using HSE_BANK.Interfaces.IFactories;

public class AnalysisFacadeTests
{
    [Fact]
    public void GetTotalAmountByDate_ShouldReturnCorrectBalance()
    {
        // Arrange
        var operationRepoMock = new Mock<IOperationRepository>();
        var testAccount = new BankAccount("Основной", 1000);
        var operations = new List<Operation>
        {
            new Operation(OperationType.Enrollments, testAccount.Id, 5000, DateTime.Now, "Зарплата", "Зарплата", CategoryType.Salary, Mock.Of<ICategoryFactory>()),
            new Operation(OperationType.Expenses, testAccount.Id, 2000, DateTime.Now, "Магазин", "Продукты", CategoryType.Supermarkets, Mock.Of<ICategoryFactory>())
        };

        operationRepoMock.Setup(r => r.GetOperationsByDateAndAccountId(testAccount.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(operations);

        var facade = new AnalysisFacade(operationRepoMock.Object);

        // Act
        var balanceDiff = facade.GetTotalAmountByDate(testAccount, DateTime.Now.AddDays(-30), DateTime.Now);

        // Assert
        Assert.Equal(3000, balanceDiff);
    }
}