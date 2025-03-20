using Xunit;
using HSE_BANK.Factories;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using System;

public class CategoryFactoryTests
{
    [Fact]
    public void CreateCategory_ShouldReturnValidCategory()
    {
        var factory = new CategoryFactory();
        string name = "Продукты";
        CategoryType type = CategoryType.Supermarkets;

        var category = factory.CreateCategory(name, type);

        Assert.NotNull(category);
        Assert.Equal(name, category.Name);
        Assert.Equal(type, category.Type);
        Assert.NotEqual(Guid.Empty, category.Id);
    }

    [Fact]
    public void CreateCategory_ShouldThrowException_WhenNameIsEmpty()
    {
        var factory = new CategoryFactory();

        Assert.Throws<ArgumentException>(() => factory.CreateCategory("", CategoryType.Salary));
    }
}