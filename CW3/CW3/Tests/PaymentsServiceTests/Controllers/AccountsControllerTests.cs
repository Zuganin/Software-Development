using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentsService.Application.Interfaces;
using PaymentsService.Controllers;
using PaymentsService.Domain.Model.Entities;
using Xunit;

namespace PaymentsServiceTests.Controllers
{
    public class AccountsControllerTests
    {
        private readonly Mock<IAccountService> _serviceMock = new();
        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            _controller = new AccountsController(_serviceMock.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenAccountCreated()
        {
            var userId = Guid.NewGuid();
            _serviceMock.Setup(s => s.CreateAccountAsync(userId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var result = await _controller.Create(userId, CancellationToken.None);
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetBalance), created.ActionName);
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenAccountExists()
        {
            var userId = Guid.NewGuid();
            _serviceMock.Setup(s => s.CreateAccountAsync(userId, It.IsAny<CancellationToken>())).Throws(new InvalidOperationException("already exists"));
            var result = await _controller.Create(userId, CancellationToken.None);
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Deposit_ReturnsOkWithBalance()
        {
            var userId = Guid.NewGuid();
            _serviceMock.Setup(s => s.DepositAsync(userId, 100, It.IsAny<CancellationToken>())).ReturnsAsync(200);
            var dto = new DepositRequest { UserId = userId, Amount = 100 };
            var result = await _controller.Deposit(dto, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result);
            // Проверяем, что OkObjectResult.Value — это объект с полем balance
            var dict = ok.Value as IDictionary<string, object>;
            if (dict != null && dict.ContainsKey("balance"))
            {
                Assert.Equal(200, dict["balance"]);
            }
            else
            {
                // Fallback: сравнение через ToString()
                Assert.Contains("200", ok.Value?.ToString());
            }
        }

        [Fact]
        public async Task GetBalance_ReturnsOkWithBalance()
        {
            var userId = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetBalanceAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(123m);
            var result = await _controller.GetBalance(userId, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(123m, Convert.ToDecimal(ok.Value));
        }

        [Fact]
        public async Task GetTransactions_ReturnsOkWithTransactions()
        {
            var userId = Guid.NewGuid();
            var txs = new List<Transaction> { new Transaction { Id = Guid.NewGuid(), AccountId = Guid.NewGuid(), Amount = 10, Type = "deposit", OccurredOn = DateTime.UtcNow } };
            _serviceMock.Setup(s => s.GetTransactionsAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(txs);
            var result = await _controller.GetTransactions(userId, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Single((List<Transaction>)ok.Value);
        }
    }
}
