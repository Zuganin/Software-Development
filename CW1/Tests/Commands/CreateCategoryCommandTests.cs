using HSE_BANK.Commands;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;
using Moq;

namespace Tests.Commands;

public class CreateCategoryCommandTests
{
    [Fact]
    public void Execute_ShouldCallCreateCategoryOnFacade()
    {
        // Arrange
        var facadeMock = new Mock<ICategoryFacade>();
        var command = new CreateCategoryCommand(facadeMock.Object);
        
        command.Create("Продукты", CategoryType.Supermarkets);

        // Act
        command.Execute();

        // Assert
        facadeMock.Verify(f => f.CreateCategory("Продукты", CategoryType.Supermarkets), Times.Once);
    }
}