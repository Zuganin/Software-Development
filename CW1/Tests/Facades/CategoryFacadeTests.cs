using Xunit;
using Moq;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using System;

public class CategoryFacadeTests
{
    [Fact]
    public void CreateCategory_ShouldCreateAndSaveCategory()
    {
        // Arrange
        var factoryMock = new Mock<ICategoryFactory>();
        var repoMock = new Mock<ICategoryRepository>();

        factoryMock.Setup(f => f.CreateCategory("Продукты", CategoryType.Supermarkets))
            .Returns(new Category("Продукты", CategoryType.Supermarkets));

        var facade = new CategoryFacade(repoMock.Object, factoryMock.Object);

        // Act
        var category = facade.CreateCategory("Продукты", CategoryType.Supermarkets);

        // Assert
        Assert.NotNull(category);
        repoMock.Verify(r => r.Add(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public void GetCategoryByName_ShouldReturnCorrectCategory()
    {
        // Arrange
        var repoMock = new Mock<ICategoryRepository>();
        var testCategory = new Category("Здоровье", CategoryType.Medicine);
        repoMock.Setup(r => r.GetByName("Здоровье")).Returns(testCategory);

        var facade = new CategoryFacade(repoMock.Object, Mock.Of<ICategoryFactory>());

        // Act
        var category = facade.GetCategory("Здоровье");

        // Assert
        Assert.NotNull(category);
        Assert.Equal(CategoryType.Medicine, category.Type);
    }
}