using Xunit;
using Moq;
using HSE_BANK.Facades;
using HSE_BANK.Commands;
using HSE_BANK.Interfaces.Repository;

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