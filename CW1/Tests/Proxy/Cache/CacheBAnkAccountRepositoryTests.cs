using HSE_BANK.Domain_Models;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Proxy.Cache;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Tests.Proxy.Cache;

public class CacheBankAccountRepositoryTests
{
    private readonly Mock<IBankAccountRepository> _realRepositoryMock;
    private readonly IMemoryCache _cache;
    private readonly CacheBankAccountRepository _cacheBankAccountRepository;

    public CacheBankAccountRepositoryTests()
    {
        _realRepositoryMock = new Mock<IBankAccountRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _cacheBankAccountRepository = new CacheBankAccountRepository(_realRepositoryMock.Object, _cache);
    }

    [Fact]
    public void GetById_ShouldReturnFromCache_WhenDataExists()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedAccount = new BankAccount("Основной счёт", 5000);
        _cache.Set("BankAccount:" + accountId, expectedAccount);

        // Act
        var result = _cacheBankAccountRepository.GetById(accountId);

        // Assert
        Assert.Equal(expectedAccount, result);
        _realRepositoryMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetById_ShouldFetchFromRepository_WhenNotInCache()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var expectedAccount = new BankAccount("Новый счёт", 10000);
        _realRepositoryMock.Setup(r => r.GetById(accountId)).Returns(expectedAccount);

        // Act
        var result = _cacheBankAccountRepository.GetById(accountId);

        // Assert
        Assert.Equal(expectedAccount, result);
        _realRepositoryMock.Verify(r => r.GetById(accountId), Times.Once);
    }

    [Fact]
    public void GetByName_ShouldReturnFromCache_WhenDataExists()
    {
        // Arrange
        var accountName = "Банковский";
        var expectedAccount = new BankAccount(accountName, 7500);
        _cache.Set("BankAccount:" + accountName, expectedAccount);

        // Act
        var result = _cacheBankAccountRepository.GetByName(accountName);

        // Assert
        Assert.Equal(expectedAccount, result);
        _realRepositoryMock.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void GetByName_ShouldFetchFromRepository_WhenNotInCache()
    {
        // Arrange
        var accountName = "Зарплатный счёт";
        var expectedAccount = new BankAccount(accountName, 20000);
        _realRepositoryMock.Setup(r => r.GetByName(accountName)).Returns(expectedAccount);

        // Act
        var result = _cacheBankAccountRepository.GetByName(accountName);

        // Assert
        Assert.Equal(expectedAccount, result);
        _realRepositoryMock.Verify(r => r.GetByName(accountName), Times.Once);
    }

    [Fact]
    public void Add_ShouldInvalidateCache()
    {
        // Arrange
        var account = new BankAccount("Депозит", 30000);

        // Act
        _cacheBankAccountRepository.Add(account);

        // Assert
        Assert.False(_cache.TryGetValue("BankAccounts", out _));
        _realRepositoryMock.Verify(r => r.Add(account), Times.Once);
    }

    [Fact]
    public void Update_ShouldInvalidateCache()
    {
        // Arrange
        var account = new BankAccount("Кредитный", 15000);

        // Act
        _cacheBankAccountRepository.Update(account);

        // Assert
        Assert.False(_cache.TryGetValue("BankAccounts", out _));
        _realRepositoryMock.Verify(r => r.Update(account), Times.Once);
    }

    [Fact]
    public void Delete_ShouldInvalidateCache()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act
        _cacheBankAccountRepository.Delete(accountId);

        // Assert
        Assert.False(_cache.TryGetValue("BankAccount:" + accountId, out _));
        _realRepositoryMock.Verify(r => r.Delete(accountId), Times.Once);
    }

    [Fact]
    public void GetAll_ShouldReturnCachedData()
    {
        // Arrange
        var accounts = new List<BankAccount>
        {
            new BankAccount("Сберегательный", 25000),
            new BankAccount("Резервный", 12000)
        };
        _cache.Set("BankAccounts", accounts);

        // Act
        var result = _cacheBankAccountRepository.GetAll();

        // Assert
        Assert.Equal(accounts, result);
        _realRepositoryMock.Verify(r => r.GetAll(), Times.Never);
    }

    [Fact]
    public void GetAll_ShouldFetchFromRepository_WhenCacheIsEmpty()
    {
        // Arrange
        var accounts = new List<BankAccount>
        {
            new BankAccount("Бонусный", 5000)
        };
        _realRepositoryMock.Setup(r => r.GetAll()).Returns(accounts);

        // Act
        var result = _cacheBankAccountRepository.GetAll();

        // Assert
        Assert.Equal(accounts, result);
        _realRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }
}