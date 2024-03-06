using OrderManagement.Interfaces;
using OrderService.Interfaces.Services;

namespace OrderService.Impl.Services;

public class OrderService : IOrderService
{
  private readonly IOrderManager _orderManager;

  public OrderService(IOrderManager orderManager)
  {
    _orderManager = orderManager;
  }

  private const int ORDER_BOOK_DEPTH = 5;

  public async Task<IList<Order>> GetOptimalOrderExecution(OrderTypeEnum orderType, decimal orderAmount)
  {
    IList<Order> orderExecutions = new List<Order>();

    List<OrderBook> orderBooks = (await _orderManager.GetOrderBooks(ORDER_BOOK_DEPTH)).ToList();

    List<OrderWrapper> orders = orderBooks
      .SelectMany(orderBooks => orderType == OrderTypeEnum.Buy ? orderBooks.Asks : orderBooks.Bids)
      .OrderBy(wrapper => orderType == OrderTypeEnum.Buy ? wrapper.Order.Price : -wrapper.Order.Price)
      .ToList();

    decimal remaingAmount = orderAmount;

    while (remaingAmount > 0 && orders.Count > 0)
    {
      Order order = orders.First().Order;
      orders.RemoveAt(0);

      if (order.Amount <= remaingAmount)
      {
        orderExecutions.Add(order);
        remaingAmount -= order.Amount;
      }
      else
      {
        orderExecutions.Add(new Order
        {
          Amount = remaingAmount,
          Price = order.Price,
          Type = order.Type
        });

        remaingAmount = 0;
      }
    }

    return orderExecutions;
  }

  // TODO: Niko: logging & error handling
}
