using OrderManagement.Interfaces.Enums;
using OrderManagement.Interfaces.Responses;

namespace OrderService.Interfaces.Services;

public interface IOrderService
{
  Task<IList<Order>> GetOptimalOrderExecution(OrderTypeEnum orderType, decimal orderAmount);
}
