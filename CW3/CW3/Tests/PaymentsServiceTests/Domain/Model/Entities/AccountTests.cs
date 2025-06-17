using System;
using Xunit;
using PaymentsService.Domain.Model.Entities;

namespace PaymentsServiceTests.Domain.Model.Entities
{
    public class AccountTests
    {
        [Fact]
        public void Deposit_IncreasesBalance()
        {
            var acc = Account.Create(Guid.NewGuid());
            acc.Deposit(100);
            Assert.Equal(100, acc.Balance);
        }

        [Fact]
        public void Withdraw_DecreasesBalance()
        {
            var acc = Account.Create(Guid.NewGuid());
            acc.Deposit(200);
            acc.Withdraw(50);
            Assert.Equal(150, acc.Balance);
        }

        [Fact]
        public void Withdraw_Throws_WhenInsufficientFunds()
        {
            var acc = Account.Create(Guid.NewGuid());
            acc.Deposit(10);
            Assert.Throws<InvalidOperationException>(() => acc.Withdraw(100));
        }

        [Fact]
        public void Withdraw_Throws_WhenAccountInactive()
        {
            var acc = Account.Create(Guid.NewGuid());
            acc.Deposit(100);
            acc.Close();
            Assert.False(acc.IsActive);
            Assert.Throws<InvalidOperationException>(() => acc.Withdraw(10));
        }

        [Fact]
        public void Deposit_DoesNothing_WhenAccountInactive()
        {
            var acc = Account.Create(Guid.NewGuid());
            acc.Close();
            acc.Deposit(100);
            Assert.Equal(0, acc.Balance);
        }

        [Fact]
        public void Close_DeactivatesAccount()
        {
            var acc = Account.Create(Guid.NewGuid());
            acc.Close();
            Assert.False(acc.IsActive);
        }

        [Fact]
        public void Close_DoesNothing_IfAlreadyInactive()
        {
            var acc = Account.Create(Guid.NewGuid());
            acc.Close();
            acc.Close();
            Assert.False(acc.IsActive);
        }

        [Fact]
        public void GetBalance_ReturnsCurrentBalance()
        {
            var acc = Account.Create(Guid.NewGuid());
            acc.Deposit(123);
            Assert.Equal(123, acc.GetBalance());
        }

        [Fact]
        public void Account_InitializesWithCorrectDefaults()
        {
            var userId = Guid.NewGuid();
            var acc = Account.Create(userId);
            Assert.Equal(userId, acc.UserId);
            Assert.True(acc.IsActive);
            Assert.Equal(0, acc.Balance);
        }
    }
}
