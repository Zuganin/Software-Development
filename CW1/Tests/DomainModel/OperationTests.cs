using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.IFactories;
using Moq;

namespace Tests.DomainModel;

public class OperationTests
{
    [Fact]
    public void Constructor_ShouldCreateOperationWithValidData()
    {
        // Arrange
        var factoryMock = new Mock<ICategoryFactory>();
        factoryMock.Setup(f => f.CreateCategory(It.IsAny<string>(), It.IsAny<CategoryType>()))
            .Returns((string name, CategoryType type) => new Category(name, type));

        string categoryName = "Зарплата";
        CategoryType categoryType = CategoryType.Salary;
        OperationType operationType = OperationType.Enrollments;
        Guid bankAccountId = Guid.NewGuid();
        decimal amount = 5000;
        DateTime date = DateTime.Now;
        string description = "Пополнение зарплаты";

        // Act
        var operation = new Operation(operationType, bankAccountId, amount, date, description, categoryName, categoryType, factoryMock.Object);

        // Assert
        Assert.NotNull(operation);
        Assert.Equal(operationType, operation.Type);
        Assert.Equal(bankAccountId, operation.BankAccountId);
        Assert.Equal(amount, operation.Amount);
        Assert.Equal(date, operation.Date);
        Assert.Equal(description, operation.Description);
        Assert.NotNull(operation.Category);
        Assert.Equal(categoryName, operation.Category.Name);
        Assert.Equal(categoryType, operation.Category.Type);
        Assert.NotEqual(Guid.Empty, operation.Id);
    }
}