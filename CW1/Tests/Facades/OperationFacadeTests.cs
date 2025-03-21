using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;
using Moq;

namespace Tests.Facades;

public class OperationFacadeTests
{
    private readonly Mock<IOperationFactory> _operationFactoryMock;
    private readonly Mock<ICategoryFactory> _categoryFactoryMock;
    private readonly Mock<IOperationRepository> _operationRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly OperationFacade _operationFacade;

    public OperationFacadeTests()
    {
        _operationFactoryMock = new Mock<IOperationFactory>();
        _categoryFactoryMock = new Mock<ICategoryFactory>();
        _operationRepositoryMock = new Mock<IOperationRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        
        _operationFacade = new OperationFacade(
            _operationFactoryMock.Object, 
            _categoryFactoryMock.Object,
            _operationRepositoryMock.Object, 
            _categoryRepositoryMock.Object);
    }

    [Fact]
    public void CreateOperation_ShouldCreateAndAddOperation()
    {
        // Arrange
        var operationType = OperationType.Expenses;
        var accountId = Guid.NewGuid();
        var amount = 500;
        var description = "Покупка";
        var date = DateTime.Now;
        var categoryName = "Еда";
        var categoryType = CategoryType.Supermarkets;
        var expectedCategory = new Category(categoryName, categoryType);
        var expectedOperation = new Operation(operationType, accountId, amount, date, description, expectedCategory);

        _operationFactoryMock
            .Setup(f => f.CreateOperation(operationType, accountId, amount, date, description, categoryName, categoryType, _categoryFactoryMock.Object))
            .Returns(expectedOperation);

        // Act
        var result = _operationFacade.CreateOperation(operationType, accountId, amount,  date, description,categoryName, categoryType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(amount, result.Amount);
        Assert.Equal(description, result.Description);
        Assert.Equal(categoryName, result.Category.Name);
        Assert.Equal(categoryType, result.Category.Type);

        _operationRepositoryMock.Verify(r => r.Add(expectedOperation), Times.Once);
        _categoryRepositoryMock.Verify(r => r.Add(expectedOperation.Category), Times.Once);
    }

    [Fact]
    public void GetAllOperations_ShouldReturnOperations()
    {
        // Arrange
        var operations = new List<Operation>
        {
            new Operation(OperationType.Enrollments, Guid.NewGuid(), 1000, DateTime.Now, "Зарплата", "Зарплата", CategoryType.Salary, _categoryFactoryMock.Object),
            new Operation(OperationType.Expenses, Guid.NewGuid(), 200, DateTime.Now, "Кафе", "Рестораны", CategoryType.Cafe, _categoryFactoryMock.Object)
        };

        _operationRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(operations);

        // Act
        var result = _operationFacade.GetAllOperations();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetOperationById_ShouldReturnCorrectOperation()
    {
        // Arrange
        
        var expectedOperation = new Operation(OperationType.Expenses, Guid.NewGuid(), 500,
            DateTime.Now, "Покупка", "Еда", CategoryType.Supermarkets, _categoryFactoryMock.Object) {};
        var operationId = expectedOperation.Id;
        _operationRepositoryMock
            .Setup(r => r.GetById(operationId))
            .Returns(expectedOperation);

        // Act
        var result = _operationFacade.GetOperationById(operationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(operationId, result.Id);
    }

    [Fact]
    public void UpdateOperation_ShouldCallUpdateOnRepository()
    {
        // Arrange
        var operation = new Operation(OperationType.Expenses, Guid.NewGuid(), 300, DateTime.Now, "Такси", "Транспорт", CategoryType.Transport, _categoryFactoryMock.Object);

        // Act
        _operationFacade.UpdateOperation(operation);

        // Assert
        _operationRepositoryMock.Verify(r => r.Update(operation), Times.Once);
    }

    [Fact]
    public void DeleteOperation_ShouldCallDeleteOnRepository()
    {
        // Arrange
        
        var operation = new Operation(OperationType.Expenses, Guid.NewGuid(), 150, DateTime.Now, "Кофе", "Кафе", CategoryType.Cafe, _categoryFactoryMock.Object);
        var operationId = operation.Id;
        // Act
        _operationFacade.DeleteOperation(operation);

        // Assert
        _operationRepositoryMock.Verify(r => r.Delete(operationId), Times.Once);
    }
}