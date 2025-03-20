using Xunit;
using Moq;
using HSE_BANK.Facades;
using HSE_BANK.Commands;
using HSE_BANK.Domain_Models.Enums;
using System;

public class CreateOperationCommandTests
{
    [Fact]
    public void Execute_ShouldCallCreateOperationOnFacade()
    {
        // Arrange
        var facadeMock = new Mock<IOperationFacade>();
        var command = new CreateOperationCommand(facadeMock.Object);
        
        var operationType = OperationType.Enrollments;
        var bankAccountId = Guid.NewGuid();
        var amount = 5000;
        var date = DateTime.Now;
        var description = "Пополнение";
        var categoryName = "Зарплата";
        var categoryType = CategoryType.Salary;
        
        command.Create(operationType, bankAccountId, amount, description, date, categoryName, categoryType);

        // Act
        command.Execute();

        // Assert
        facadeMock.Verify(f => f.CreateOperation(operationType, bankAccountId, amount, description, date, categoryName, categoryType), Times.Once);
    }
}