using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;
using Moq;

namespace Tests.Facades;

public class CategoryFacadeTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ICategoryFactory> _categoryFactoryMock;
    private readonly CategoryFacade _categoryFacade;

    public CategoryFacadeTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _categoryFactoryMock = new Mock<ICategoryFactory>();
        _categoryFacade = new CategoryFacade(_categoryRepositoryMock.Object, _categoryFactoryMock.Object);
    }

    [Fact]
    public void CreateCategory_ShouldCreateAndAddCategory()
    {
        // Arrange
        var categoryName = "Продукты";
        var categoryType = CategoryType.Supermarkets;
        var expectedCategory = new Category(categoryName, categoryType);

        _categoryFactoryMock
            .Setup(f => f.CreateCategory(categoryName, categoryType))
            .Returns(expectedCategory);

        // Act
        var result = _categoryFacade.CreateCategory(categoryName, categoryType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryName, result.Name);
        Assert.Equal(categoryType, result.Type);

        _categoryRepositoryMock.Verify(r => r.Add(expectedCategory), Times.Once);
    }

    [Fact]
    public void GetAllCategoryTypes_ShouldReturnAllCategoryTypes()
    {
        // Act
        var result = _categoryFacade.GetAllCategoryTypes();

        // Assert
        var expectedTypes = Enum.GetValues<CategoryType>().ToList();
        Assert.Equal(expectedTypes, result);
    }

    [Fact]
    public void GetAllCategories_ShouldReturnCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category("Зарплата", CategoryType.Salary),
            new Category("Кафе", CategoryType.Cafe)
        };

        _categoryRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(categories);

        // Act
        var result = _categoryFacade.GetAllCategories();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetCategory_ByName_ShouldReturnCorrectCategory()
    {
        // Arrange
        var categoryName = "Кафе";
        var expectedCategory = new Category(categoryName, CategoryType.Cafe);

        _categoryRepositoryMock
            .Setup(r => r.GetByName(categoryName))
            .Returns(expectedCategory);

        // Act
        var result = _categoryFacade.GetCategory(categoryName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryName, result.Name);
    }

    [Fact]
    public void GetCategory_ById_ShouldReturnCorrectCategory()
    {
        // Arrange
       
        var expectedCategory = new Category("Еда", CategoryType.Supermarkets);
        var categoryId = expectedCategory.Id;
        _categoryRepositoryMock
            .Setup(r => r.GetById(categoryId))
            .Returns(expectedCategory);

        // Act
        var result = _categoryFacade.GetCategory(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
    }

    [Fact]
    public void UpdateCategory_ShouldCallUpdateOnRepository()
    {
        // Arrange
        var category = new Category("Спорт", CategoryType.Entertainment);

        // Act
        _categoryFacade.UpdateCategory(category);

        // Assert
        _categoryRepositoryMock.Verify(r => r.Update(category), Times.Once);
    }

    [Fact]
    public void DeleteCategory_ShouldCallDeleteOnRepository()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        _categoryFacade.DeleteCategory(categoryId);

        // Assert
        _categoryRepositoryMock.Verify(r => r.Delete(categoryId), Times.Once);
    }
}