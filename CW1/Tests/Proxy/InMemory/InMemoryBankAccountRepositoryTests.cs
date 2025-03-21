using HSE_BANK.Domain_Models;
using HSE_BANK.Proxy.InMemory;

namespace Tests.Proxy.InMemory;

public class InMemoryBankAccountRepositoryTests
{
    private readonly InMemoryBankAccountRepository _repository;

    public InMemoryBankAccountRepositoryTests()
    {
        _repository = new InMemoryBankAccountRepository();
    }

    [Fact]
    public void GetById_ShouldReturnAccount_WhenExists()
    {
        // Arrange
        var account = new BankAccount("Основной счет", 5000);
        _repository.Add(account);

        // Act
        var result = _repository.GetById(account.Id);

        // Assert
        Assert.Equal(account, result);
    }

    [Fact]
    public void GetById_ShouldThrowException_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _repository.GetById(accountId));
        Assert.Contains("Bank account with ID", exception.Message);
    }

    [Fact]
    public void GetByName_ShouldReturnAccount_WhenExists()
    {
        // Arrange
        var account = new BankAccount("Резервный счет", 10000);
        _repository.Add(account);

        // Act
        var result = _repository.GetByName("Резервный счет");

        // Assert
        Assert.Equal(account, result);
    }

    [Fact]
    public void GetByName_ShouldThrowException_WhenAccountDoesNotExist()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _repository.GetByName("Несуществующий счет"));
        Assert.Contains("Bank account with name", exception.Message);
    }

    [Fact]
    public void Add_ShouldAddAccount_WhenValid()
    {
        // Arrange
        var account = new BankAccount("Зарплатный счет", 15000);

        // Act
        _repository.Add(account);
        var result = _repository.GetById(account.Id);

        // Assert
        Assert.Equal(account, result);
    }

    [Fact]
    public void Add_ShouldThrowException_WhenAccountIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Add(null));
    }

    [Fact]
    public void Add_ShouldThrowException_WhenAccountAlreadyExists()
    {
        // Arrange
        var account = new BankAccount("Сберегательный счет", 20000);
        _repository.Add(account);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _repository.Add(account));
        Assert.Contains("Bank account with ID", exception.Message);
    }

    [Fact]
    public void Update_ShouldUpdateAccount_WhenExists()
    {
        // Arrange
        var account = new BankAccount("Кредитный счет", 3000);
        _repository.Add(account);

        account.UpdateBalance(5000);

        // Act
        _repository.Update(account);
        var result = _repository.GetById(account.Id);

        // Assert
        Assert.Equal(5000, result.Balance);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenAccountIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Update(null));
    }

    [Fact]
    public void Update_ShouldThrowException_WhenAccountDoesNotExist()
    {
        // Arrange
        var account = new BankAccount("Несуществующий счет", 7000);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _repository.Update(account));
        Assert.Contains("Bank account with ID", exception.Message);
    }

    [Fact]
    public void Delete_ShouldRemoveAccount_WhenExists()
    {
        // Arrange
        var account = new BankAccount("Депозитный счет", 25000);
        _repository.Add(account);

        // Act
        _repository.Delete(account.Id);

        // Assert
        Assert.Throws<ArgumentException>(() => _repository.GetById(account.Id));
    }

    [Fact]
    public void Delete_ShouldThrowException_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _repository.Delete(accountId));
        Assert.Contains("Bank account with ID", exception.Message);
    }

    [Fact]
    public void GetAll_ShouldReturnAllAccounts()
    {
        // Arrange
        var account1 = new BankAccount("Бизнес счет", 50000);
        var account2 = new BankAccount("Инвестиционный счет", 75000);

        _repository.Add(account1);
        _repository.Add(account2);

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(2, result.Count());
    }
}