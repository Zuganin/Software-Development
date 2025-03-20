using Xunit;
using HSE_BANK.Domain_Models;
using System;
namespace Tests;

public class BancAccountTests
{
    [Fact]
    public void Constructor_ShouldCreateBankAccountWithValidData()
    {
        // Arrange
        string name = "Основной счет";
        decimal balance = 1000;

        // Act
        var account = new BankAccount(name, balance);

        // Assert
        Assert.NotNull(account);
        Assert.Equal(name, account.Name);
        Assert.Equal(balance, account.Balance);
        Assert.NotEqual(Guid.Empty, account.Id);
    }
}