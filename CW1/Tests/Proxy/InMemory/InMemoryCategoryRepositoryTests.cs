using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Proxy.InMemory;

namespace Tests.Proxy.InMemory;

public class InMemoryCategoryRepositoryTests
{
    private readonly InMemoryCategoryRepository _repository;

    public InMemoryCategoryRepositoryTests()
    {
        _repository = new InMemoryCategoryRepository();
    }

    [Fact]
    public void GetById_ShouldReturnCategory_WhenExists()
    {
        // Arrange
        var category = new Category("Продукты", CategoryType.Supermarkets);
        _repository.Add(category);

        // Act
        var result = _repository.GetById(category.Id);

        // Assert
        Assert.Equal(category, result);
    }

    [Fact]
    public void GetById_ShouldThrowException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _repository.GetById(categoryId));
        Assert.Contains("Category with ID", exception.Message);
    }

    [Fact]
    public void GetByName_ShouldReturnCategory_WhenExists()
    {
        // Arrange
        var category = new Category("Рестораны", CategoryType.Cafe);
        _repository.Add(category);

        // Act
        var result = _repository.GetByName("Рестораны");

        // Assert
        Assert.Equal(category, result);
    }

    [Fact]
    public void GetByName_ShouldThrowException_WhenCategoryDoesNotExist()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _repository.GetByName("Несуществующая категория"));
    }

    [Fact]
    public void Add_ShouldAddCategory_WhenValid()
    {
        // Arrange
        var category = new Category("Развлечения", CategoryType.Entertainment);

        // Act
        _repository.Add(category);
        var result = _repository.GetById(category.Id);

        // Assert
        Assert.Equal(category, result);
    }

    [Fact]
    public void Add_ShouldThrowException_WhenCategoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Add(null));
    }

    [Fact]
    public void Add_ShouldThrowException_WhenCategoryAlreadyExists()
    {
        // Arrange
        var category = new Category("Спорт", CategoryType.Hobbies);
        _repository.Add(category);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _repository.Add(category));
        Assert.Contains("Category with ID", exception.Message);
    }

    [Fact]
    public void Update_ShouldUpdateCategory_WhenExists()
    {
        // Arrange
        var category = new Category("Здоровье", CategoryType.Medicine);
        _repository.Add(category);

        var updatedCategory = category.Update("Медицина", CategoryType.Medicine);

        // Act
        _repository.Update(updatedCategory);
        var result = _repository.GetById(category.Id);

        // Assert
        Assert.Equal("Медицина", result.Name);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenCategoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Update(null));
    }

    [Fact]
    public void Update_ShouldThrowException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var category = new Category("Несуществующая", CategoryType.Entertainment);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _repository.Update(category));
        Assert.Contains("Category with ID", exception.Message);
    }

    [Fact]
    public void Delete_ShouldRemoveCategory_WhenExists()
    {
        // Arrange
        var category = new Category("Путешествия", CategoryType.Entertainment);
        _repository.Add(category);

        // Act
        _repository.Delete(category.Id);

        // Assert
        Assert.Throws<ArgumentException>(() => _repository.GetById(category.Id));
    }

    [Fact]
    public void Delete_ShouldThrowException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _repository.Delete(categoryId));
        Assert.Contains("Category with ID", exception.Message);
    }

    [Fact]
    public void GetAll_ShouldReturnAllCategories()
    {
        // Arrange
        var category1 = new Category("Транспорт", CategoryType.Transport);
        var category2 = new Category("Кино", CategoryType.Entertainment);

        _repository.Add(category1);
        _repository.Add(category2);

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetByType_ShouldReturnFilteredCategories()
    {
        // Arrange
        var category1 = new Category("Супермаркеты", CategoryType.Supermarkets);
        var category2 = new Category("Рестораны", CategoryType.Cafe);
        var category3 = new Category("Фитнес", CategoryType.Hobbies);

        _repository.Add(category1);
        _repository.Add(category2);
        _repository.Add(category3);

        // Act
        var result = _repository.GetByType(CategoryType.Hobbies);

        // Assert
        Assert.Single(result);
        Assert.Equal("Фитнес", result.First().Name);
    }
}