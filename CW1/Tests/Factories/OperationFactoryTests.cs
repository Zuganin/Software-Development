using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Factories;
using HSE_BANK.Interfaces.IFactories;
using Moq;

namespace Tests.Factories;

public class OperationFactoryTests
{
    [Fact]
    public void CreateOperation_ShouldReturnValidOperation()
    {
        var factory = new OperationFactory();
        var categoryFactoryMock = new Mock<ICategoryFactory>();
        categoryFactoryMock.Setup(f => f.CreateCategory(It.IsAny<string>(), It.IsAny<CategoryType>()))
            .Returns((string name, CategoryType type) => new Category(name, type));

        OperationType type = OperationType.Enrollments;
        Guid bankAccountId = Guid.NewGuid();
        decimal amount = 5000;
        DateTime date = DateTime.Now;
        string description = "Пополнение";
        string categoryName = "Зарплата";
        CategoryType categoryType = CategoryType.Salary;

        var operation = factory.CreateOperation(type, bankAccountId, amount, date, description, categoryName, categoryType, categoryFactoryMock.Object);

        Assert.NotNull(operation);
        Assert.Equal(type, operation.Type);
        Assert.Equal(bankAccountId, operation.BankAccountId);
        Assert.Equal(amount, operation.Amount);
        Assert.Equal(date, operation.Date);
        Assert.Equal(description, operation.Description);
        Assert.NotEqual(Guid.Empty, operation.Id);
    }

    [Fact]
    public void CreateOperation_ShouldThrowException_WhenAmountIsZeroOrNegative()
    {
        var factory = new OperationFactory();
        var categoryFactoryMock = new Mock<ICategoryFactory>();

        Assert.Throws<ArgumentException>(() => factory.CreateOperation(OperationType.Expenses, Guid.NewGuid(), 0, DateTime.Now, "Покупка", "Еда", CategoryType.Supermarkets, categoryFactoryMock.Object));
    }

    [Fact]
    public void CreateOperation_ShouldSetDefaultDescription_WhenDescriptionIsEmpty()
    {
        var factory = new OperationFactory();
        var categoryFactoryMock = new Mock<ICategoryFactory>();
        categoryFactoryMock.Setup(f => f.CreateCategory(It.IsAny<string>(), It.IsAny<CategoryType>()))
            .Returns((string name, CategoryType type) => new Category(name, type));

        OperationType type = OperationType.Enrollments;
        Guid bankAccountId = Guid.NewGuid();
        decimal amount = 5000;
        DateTime date = DateTime.Now;
        string categoryName = "Зарплата";
        CategoryType categoryType = CategoryType.Salary;

        var operation = factory.CreateOperation(type, bankAccountId, amount, date, "", categoryName, categoryType, categoryFactoryMock.Object);
        
        Assert.Equal("Нет описания операции", operation.Description);
    }
}