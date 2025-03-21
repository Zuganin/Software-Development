using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;

namespace Tests.DomainModel;

public class CategoryTests
{
    [Fact]
    public void Constructor_ShouldCreateCategoryWithValidData()
    {
        // Arrange
        string name = "Продукты";
        CategoryType type = CategoryType.Supermarkets;

        // Act
        var category = new Category(name, type);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(name, category.Name);
        Assert.Equal(type, category.Type);
        Assert.NotEqual(Guid.Empty, category.Id);
    }
}