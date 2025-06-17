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

        /// <summary>
        /// Создать заказ.
        /// </summary>
        /// <param name="request">Данные заказа</param>
        /// <response code="200">Заказ создан</response>
        /// <response code="400">Некорректная сумма</response>
        /// <example>
        /// {
        ///   "userId": "b1a7e7e2-1c2d-4b7a-9c1a-2e7e7e7e7e7",
        ///   "amount": 500.0,
        ///   "description": "Покупка товара"
        /// }
        /// </example>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than zero");
            var order = await _orderService.CreateOrderAsync(request.UserId, request.Amount, request.Description);
            return Ok(order);
        }

        /// <summary>
        /// Получить список всех заказов.
        /// </summary>
        /// <response code="200">Список заказов</response>
        /// <example>
        /// [
        ///   {
        ///     "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        ///     "userId": "b1a7e7e2-1c2d-4b7a-9c1a-2e7e7e7e7e7",
        ///     "amount": 500.0,
        ///     "description": "Покупка товара",
        ///     "status": 0
        ///   }
        /// ]
        /// </example>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Получить заказ по идентификатору.
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <response code="200">Данные заказа</response>
        /// <response code="404">Заказ не найден</response>
        /// <example>
        /// {
        ///   "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        ///   "userId": "b1a7e7e2-1c2d-4b7a-9c1a-2e7e7e7e7e7e",
        ///   "amount": 500.0,
        ///   "description": "Покупка товара",
        ///   "status": 1
        /// }
        /// </example>
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
