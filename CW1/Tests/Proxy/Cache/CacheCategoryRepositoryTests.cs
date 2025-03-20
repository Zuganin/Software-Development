using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using HSE_BANK.Proxy.Cache;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using System;
using System.Collections.Generic;

public class CacheCategoryRepositoryTests
{
    [Fact]
    public void GetById_ShouldReturnFromCache_WhenExists()
    {
        // Arrange
        var repoMock = new Mock<ICategoryRepository>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var repository = new CacheCategoryRepository(repoMock.Object, cache);

        var category = new Category("Еда", CategoryType.Supermarkets);
        cache.Set("Category:" + category.Id, category);

        // Act
        var result = repository.GetById(category.Id);

        // Assert
        Assert.Equal(category, result);
        repoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetByType_ShouldReturnCorrectCategories()
    {
        // Arrange
        var repoMock = new Mock<ICategoryRepository>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var repository = new CacheCategoryRepository(repoMock.Object, cache);

        var categories = new List<Category>
        {
            new Category("Зарплата", CategoryType.Salary),
            new Category("Фриланс", CategoryType.Salary)
        };
        repoMock.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = repository.GetByType(CategoryType.Salary);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Equal(CategoryType.Salary, c.Type));
    }
}