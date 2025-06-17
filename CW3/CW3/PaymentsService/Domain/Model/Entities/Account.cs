using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Domain.Model.Entities;

public class Account
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Balance { get; private set; }
    public bool IsActive { get; private set; }

    // Конструктор для EF Core
    private Account() { }

    public static Account Create(Guid userId)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Balance = 0,
            IsActive = true
        };
        return account;
    }

    // Пополнение баланса
    public void Deposit(decimal amount)
    {
        if (amount > 0 && IsActive)
        {
            Balance += amount;
        }
    }

    // Снятие со счёта
    public void Withdraw(decimal amount)
    {
        if (amount > 0 && IsActive && Balance >= amount)
        {
            Balance -= amount;
        }
        else
        {
            throw new InvalidOperationException("Insufficient funds or inactive account");
        }
    }

    // Закрытие аккаунта
    public void Close()
    {
        if (IsActive)
        {
            IsActive = false;
        }
    }

    // Получение текущего баланса
    public decimal GetBalance() => Balance;
}