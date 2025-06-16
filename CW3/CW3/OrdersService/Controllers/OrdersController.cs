using Microsoft.AspNetCore.Mvc;
using OrdersService.Application.Services;
using OrdersService.Domain.Entities;

namespace OrdersService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than zero");
            var order = await _orderService.CreateOrderAsync(request.UserId, request.Amount, request.Description);
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }
    }

    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
