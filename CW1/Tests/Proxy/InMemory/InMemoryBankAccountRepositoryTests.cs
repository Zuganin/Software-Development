using Xunit;
using Moq;
using HSE_BANK.Facades;
using HSE_BANK.Proxy.InMemory;
using HSE_BANK.Domain_Models;
using System;
using HSE_BANK.Factories;
using HSE_BANK.Interfaces.IFactories;

public class InMemoryBankAccountRepositoryTests
{
    [Fact]
    public void Add_ShouldStoreAccount()
    {
        // Arrange
        var repository = new InMemoryBankAccountRepository();
        var factory = new BankAccountFactory();
        var facade = new BankAccountFacade(factory, repository);
        var account = new BankAccount("Основной", 1000);

        // Act
        facade.CreateBankAccount(account.Name, account.Balance);
        var result = repository.GetById(account.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Основной", result.Name);
    }

    [Fact]
    public void Update_ShouldModifyAccountBalance()
    {
        // Arrange
        var repository = new InMemoryBankAccountRepository();
        var facade = new BankAccountFacade(Mock.Of<IBankAccountFactory>(), repository);
        var account = new BankAccount("Счет", 2000);
        repository.Add(account);

        // Act
        var newAccount = new BankAccount(account.Name, 3000);
        facade.UpdateBankAccount(newAccount); // Используем фасад для обновления
        var updated = repository.GetById(account.Id);

        // Assert
        Assert.Equal(3000, updated.Balance);
    }

    [Fact]
    public void Delete_ShouldRemoveAccount()
    {
        // Arrange
        var repository = new InMemoryBankAccountRepository();
        var facade = new BankAccountFacade(Mock.Of<IBankAccountFactory>(), repository);
        var account = new BankAccount("Удаляемый", 1000);
        repository.Add(account);

        // Act
        facade.DeleteBankAccount(account);

        // Assert
        Assert.Throws<ArgumentException>(() => repository.GetById(account.Id));
    }
}