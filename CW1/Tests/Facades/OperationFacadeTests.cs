using Xunit;
using Moq;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using System;

public class OperationFacadeTests
{
    [Fact]
    public void CreateOperation_ShouldCreateAndSaveOperation()
    {
        // Arrange
        var operationFactoryMock = new Mock<IOperationFactory>();
        var categoryFactoryMock = new Mock<ICategoryFactory>();
        var operationRepoMock = new Mock<IOperationRepository>();
        var categoryRepoMock = new Mock<ICategoryRepository>();

        var testOperation = new Operation(OperationType.Enrollments, Guid.NewGuid(), 5000, DateTime.Now, "Зарплата", "Зарплата", CategoryType.Salary, categoryFactoryMock.Object);
        operationFactoryMock.Setup(f => f.CreateOperation(
                It.IsAny<OperationType>(), It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<DateTime>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CategoryType>(), It.IsAny<ICategoryFactory>()))
            .Returns(testOperation);

        var facade = new OperationFacade(operationFactoryMock.Object, categoryFactoryMock.Object, operationRepoMock.Object, categoryRepoMock.Object);

        // Act
        var operation = facade.CreateOperation(OperationType.Enrollments, Guid.NewGuid(), 5000, "Зарплата", DateTime.Now, "Зарплата", CategoryType.Salary);

        // Assert
        Assert.NotNull(operation);
        operationRepoMock.Verify(r => r.Add(It.IsAny<Operation>()), Times.Once);
    }
}