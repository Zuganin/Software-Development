using HSE_BANK.Factories;

namespace Tests.Factories;

public class BankAccountFactoryTests
{
    [Fact]
    public void CreateBankAccount_ShouldReturnValidAccount()
    {
        // Arrange
        var factory = new BankAccountFactory();
        string name = "Основной счет";
        decimal balance = 1000;

        // Act
        var account = factory.CreateBankAccount(name, balance);

        // Assert
        Assert.NotNull(account);
        Assert.Equal(name, account.Name);
        Assert.Equal(balance, account.Balance);
        Assert.NotEqual(Guid.Empty, account.Id);
    }

    [Fact]
    public void CreateBankAccount_ShouldThrowException_WhenNameIsEmpty()
    {
        // Arrange
        var factory = new BankAccountFactory();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => factory.CreateBankAccount("", 1000));
    }
}