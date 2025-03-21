using HSE_BANK.Commands;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;
using Moq;

namespace Tests.Commands;

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
        
        command.Create(operationType, bankAccountId, amount,  date, description,categoryName, categoryType);

        // Act
        command.Execute();

        // Assert
        facadeMock.Verify(f => f.CreateOperation(operationType, bankAccountId, amount,  date, description,categoryName, categoryType), Times.Once);
    }
}