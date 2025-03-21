using HSE_BANK.Commands;
using HSE_BANK.Facades;
using Moq;

namespace Tests.Commands;

public class CreateBankAccountCommandTests
{
    [Fact]
    public void Execute_ShouldCallCreateBankAccountOnFacade()
    {
        // Arrange
        var facadeMock = new Mock<IBankAccountFacade>();
        var command = new CreateBankAccountCommand(facadeMock.Object);
        
        command.Create("Основной", 1000);

        // Act
        command.Execute();

        // Assert
        facadeMock.Verify(f => f.CreateBankAccount("Основной", 1000), Times.Once);
    }
}