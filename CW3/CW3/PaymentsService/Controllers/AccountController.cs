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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Guid userId, CancellationToken ct)
    {
        await _service.CreateAccountAsync(userId, ct);
        return CreatedAtAction(nameof(GetBalance), new { userId }, null);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest dto, CancellationToken ct)
    {
        await _service.DepositAsync(dto.UserId, dto.Amount, ct);
        return Ok();
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest dto, CancellationToken ct)
    {
        await _service.WithdrawAsync(dto.UserId, dto.Amount, ct);
        return Ok();
    }

    [HttpGet("{userId}/balance")]
    public async Task<ActionResult<decimal>> GetBalance(Guid userId, CancellationToken ct)
    {
        var balance = await _service.GetBalanceAsync(userId, ct);
        return Ok(balance);
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
