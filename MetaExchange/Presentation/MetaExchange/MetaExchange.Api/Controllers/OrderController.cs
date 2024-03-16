using Microsoft.AspNetCore.Mvc;
using OrderManagement.Interfaces.Enums;
using OrderManagement.Interfaces.Responses;
using OrderService.Interfaces.Services;
using System.Net.Mime;

namespace MetaExchange.Api.Controllers
{
  [ApiController]
  [Route("[controller]")]
  [Consumes(MediaTypeNames.Application.Json)]
  [Produces(MediaTypeNames.Application.Json)]
  public class OrderController : ControllerBase
  {
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;

    public OrderController(ILogger<OrderController> logger, IOrderService orderService)
    {
      _logger = logger;
      _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IList<Order>>> GetOrderExecution(OrderTypeEnum orderType, decimal orderAmount, CancellationToken cancellationToken)
    {
      _logger.LogInformation($"GetOrderExecution endpoint called with order type: {orderType} and order amount: {orderAmount}");
      var orders = await _orderService.GetOptimalOrderExecution(orderType, orderAmount, cancellationToken);
      return Ok(orders);
    }
  }
}
