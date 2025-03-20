using Xunit;
using HSE_BANK.Proxy.InMemory;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using System;

public class InMemoryCategoryRepositoryTests
{
    [Fact]
    public void Add_ShouldStoreCategory()
    {
        // Arrange
        var repository = new InMemoryCategoryRepository();
        var category = new Category("Продукты", CategoryType.Supermarkets);

        // Act
        repository.Add(category);
        var result = repository.GetById(category.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Продукты", result.Name);
    }

    [Fact]
    public void GetByType_ShouldReturnCorrectCategories()
    {
        // Arrange
        var repository = new InMemoryCategoryRepository();
        repository.Add(new Category("Зарплата", CategoryType.Salary));
        repository.Add(new Category("Фриланс", CategoryType.Salary));

        // Act
        var result = repository.GetByType(CategoryType.Salary);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void Delete_ShouldRemoveCategory()
    {
        // Arrange
        var repository = new InMemoryCategoryRepository();
        var category = new Category("Удаляемая", CategoryType.Medicine);
        repository.Add(category);

        // Act
        repository.Delete(category.Id);

        // Assert
        Assert.Throws<ArgumentException>(() => repository.GetById(category.Id));
    }
}