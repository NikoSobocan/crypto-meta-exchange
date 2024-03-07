using OrderManagement.Interfaces;

namespace OrderService.Interfaces.Services;

public interface IOrderService
{
  Task<IList<Order>> GetOptimalOrderExecution(OrderTypeEnum orderType, decimal orderAmount);
}
