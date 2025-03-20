using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using HSE_BANK.Proxy.Cache;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Domain_Models;
using System;
using System.Collections.Generic;

public class CacheBankAccountRepositoryTests
{
    [Fact]
    public void GetById_ShouldReturnFromCache_WhenExists()
    {
        // Arrange
        var repoMock = new Mock<IBankAccountRepository>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var repository = new CacheBankAccountRepository(repoMock.Object, cache);

        var account = new BankAccount("Основной", 1000);
        cache.Set("BankAccount:" + account.Id, account);

        // Act
        var result = repository.GetById(account.Id);

        // Assert
        Assert.Equal(account, result);
        repoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetById_ShouldCallRealRepo_WhenNotInCache()
    {
        // Arrange
        var repoMock = new Mock<IBankAccountRepository>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var repository = new CacheBankAccountRepository(repoMock.Object, cache);

        var account = new BankAccount("Основной", 1000);
        repoMock.Setup(r => r.GetById(account.Id)).Returns(account);

        // Act
        var result = repository.GetById(account.Id);

        // Assert
        Assert.Equal(account, result);
        repoMock.Verify(r => r.GetById(account.Id), Times.Once);
    }
}