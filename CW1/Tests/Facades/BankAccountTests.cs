using Xunit;
using Moq;
using HSE_BANK.Facades;
using HSE_BANK.Interfaces.IFactories;
using HSE_BANK.Interfaces.Repository;
using HSE_BANK.Domain_Models;
using System;
using System.Collections.Generic;

public class BankAccountFacadeTests
{
    [Fact]
    public void CreateBankAccount_ShouldCreateAndSaveAccount()
    {
        // Arrange
        var factoryMock = new Mock<IBankAccountFactory>();
        var repoMock = new Mock<IBankAccountRepository>();

        factoryMock.Setup(f => f.CreateBankAccount("Основной", 1000))
            .Returns(new BankAccount("Основной", 1000));

        var facade = new BankAccountFacade(factoryMock.Object, repoMock.Object);

        // Act
        var account = facade.CreateBankAccount("Основной", 1000);

        // Assert
        Assert.NotNull(account);
        repoMock.Verify(r => r.Add(It.IsAny<BankAccount>()), Times.Once);
    }

    [Fact]
    public void GetBankAccountById_ShouldReturnCorrectAccount()
    {
        // Arrange
        var repoMock = new Mock<IBankAccountRepository>();
        var testAccount = new BankAccount("Сбережения", 5000);
        repoMock.Setup(r => r.GetById(testAccount.Id)).Returns(testAccount);

        var facade = new BankAccountFacade(Mock.Of<IBankAccountFactory>(), repoMock.Object);

        // Act
        var account = facade.GetBankAccount(testAccount.Id);

        // Assert
        Assert.NotNull(account);
        Assert.Equal("Сбережения", account.Name);
    }
}