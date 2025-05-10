using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Proxy.Cache;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Tests.Proxy.Cache;

public class CacheCategoryRepositoryTests
{
    private readonly Mock<ICategoryRepository> _realRepositoryMock;
    private readonly IMemoryCache _cache;
    private readonly CacheCategoryRepository _cacheCategoryRepository;

    public CacheCategoryRepositoryTests()
    {
        _realRepositoryMock = new Mock<ICategoryRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _cacheCategoryRepository = new CacheCategoryRepository(_realRepositoryMock.Object, _cache);
    }

    [Fact]
    public void GetById_ShouldReturnFromCache_WhenDataExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var expectedCategory = new Category("Продукты", CategoryType.Supermarkets);
        _cache.Set("Category:" + categoryId, expectedCategory);

        // Act
        var result = _cacheCategoryRepository.GetById(categoryId);

        // Assert
        Assert.Equal(expectedCategory, result);
        _realRepositoryMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetById_ShouldFetchFromRepository_WhenNotInCache()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var expectedCategory = new Category("Развлечения", CategoryType.Entertainment);
        _realRepositoryMock.Setup(r => r.GetById(categoryId)).Returns(expectedCategory);

        // Act
        var result = _cacheCategoryRepository.GetById(categoryId);

        // Assert
        Assert.Equal(expectedCategory, result);
        _realRepositoryMock.Verify(r => r.GetById(categoryId), Times.Once);
    }

    [Fact]
    public void Add_ShouldInvalidateCache()
    {
        // Arrange
        var category = new Category("Транспорт", CategoryType.Transport);

        // Act
        _cacheCategoryRepository.Add(category);

        // Assert
        Assert.False(_cache.TryGetValue("AllCategories", out _));
        _realRepositoryMock.Verify(r => r.Add(category), Times.Once);
    }

    [Fact]
    public void GetAll_ShouldReturnCachedData()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category("Еда", CategoryType.Supermarkets),
            new Category("Зарплата", CategoryType.Salary)
        };
        _cache.Set("AllCategories", categories);

        // Act
        var result = _cacheCategoryRepository.GetAll();

        // Assert
        Assert.Equal(categories, result);
        _realRepositoryMock.Verify(r => r.GetAll(), Times.Never);
    }

    [Fact]
    public void GetAll_ShouldFetchFromRepository_WhenCacheIsEmpty()
    {
        // Arrange
        var categories = new List<Category> { new Category("Инвестиции", CategoryType.Investment) };
        _realRepositoryMock.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = _cacheCategoryRepository.GetAll();

        // Assert
        Assert.Equal(categories, result);
        _realRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void Delete_ShouldInvalidateCache()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        _cacheCategoryRepository.Delete(categoryId);

        // Assert
        Assert.False(_cache.TryGetValue("Category:" + categoryId, out _));
        _realRepositoryMock.Verify(r => r.Delete(categoryId), Times.Once);
    }
}