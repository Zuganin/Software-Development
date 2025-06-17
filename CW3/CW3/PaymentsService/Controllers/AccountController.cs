using Microsoft.AspNetCore.Mvc;
using PaymentsService.Application.Interfaces;

namespace PaymentsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _service;

    public AccountsController(IAccountService service)
    {
        _service = service;
    }

    /// <summary>
    /// Создать счёт для пользователя.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="ct">Токен отмены</param>
    /// <response code="201">Счёт создан</response>
    /// <response code="409">Счёт уже существует</response>
    /// <example>"b1a7e7e2-1c2d-4b7a-9c1a-2e7e7e7e7e"</example>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Guid userId, CancellationToken ct)
    {
        try
        {
            await _service.CreateAccountAsync(userId, ct);
            return CreatedAtAction(nameof(GetBalance), new { userId }, null);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict("Account already exists for this user");
        }
    }

    /// <summary>
    /// Пополнить счёт пользователя.
    /// </summary>
    /// <param name="dto">Данные для пополнения</param>
    /// <param name="ct">Токен отмены</param>
    /// <response code="200">Баланс после пополнения</response>
    /// <example>
    /// {
    ///   "userId": "b1a7e7e2-1c2d-4b7a-9c1a-2e7e7e7e7e7e",
    ///   "amount": 1000.0
    /// }
    /// </example>
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest dto, CancellationToken ct)
    {
        var balance = await _service.DepositAsync(dto.UserId, dto.Amount, ct);
        return Ok(new { balance });
    }

    /// <summary>
    /// Снять средства со счёта пользователя.
    /// </summary>
    /// <param name="dto">Данные для снятия</param>
    /// <param name="ct">Токен отмены</param>
    /// <response code="200">Баланс после снятия</response>
    /// <response code="400">Недостаточно средств или неактивный счёт</response>
    /// <example>
    /// {
    ///   "userId": "b1a7e7e2-1c2d-4b7a-9c1a-2e7e7e7e7e7e",
    ///   "amount": 500.0
    /// }
    /// </example>
    /// [HttpPost("withdraw")]
    /// public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest dto, CancellationToken ct)
    /// {
    ///     var balance = await _service.WithdrawAsync(dto.UserId, dto.Amount, ct);
    ///     return Ok(balance);
    /// }
    /// </example>
    [HttpGet("{userId}/balance")]
    public async Task<ActionResult<decimal>> GetBalance(Guid userId, CancellationToken ct)
    {
        var balance = await _service.GetBalanceAsync(userId, ct);
        return Ok(balance);
    }

    /// <summary>
    /// Получить историю транзакций пользователя.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="ct">Токен отмены</param>
    /// <response code="200">Список транзакций</response>
    /// <example>
    /// [
    ///   {
    ///     "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    ///     "accountId": "b1a7e7e2-1c2d-4b7a-9c1a-2e7e7e7e7e7e",
    ///     "amount": 1000.0,
    ///     "occurredOn": "2025-06-16T12:00:00Z",
    ///     "type": "deposit",
    ///     "description": "Пополнение счёта",
    ///     "idempotencyKey": null
    ///   }
    /// ]
    /// </example>
    [HttpGet("{userId}/transactions")]
    public async Task<ActionResult<List<PaymentsService.Domain.Model.Entities.Transaction>>> GetTransactions(Guid userId, CancellationToken ct)
    {
        var transactions = await _service.GetTransactionsAsync(userId, ct);
        return Ok(transactions);
    }
}

public class DepositRequest
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
}

public class WithdrawRequest
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
}
