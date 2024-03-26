using Microsoft.Extensions.Logging;
using OrderManagement.Interfaces;
using OrderManagement.Interfaces.Enums;
using OrderManagement.Interfaces.Responses;
using OrderService.Interfaces.Services;

namespace OrderService.Impl.Services;

public class OrderService : IOrderService
{
  private readonly IOrderManager _orderManager;
  private readonly ILogger<OrderService> _logger;

  public OrderService(IOrderManager orderManager, ILogger<OrderService> logger)
  {
    _orderManager = orderManager;
    _logger = logger;
  }

  private const int ORDER_BOOK_DEPTH = 3;

  public async Task<IList<Order>> GetOptimalOrderExecution(OrderTypeEnum orderType, decimal orderAmountInBTC, CancellationToken cancellationToken)
  {
    if (_logger.IsEnabled(LogLevel.Debug))
    {
      _logger.LogDebug($"GetOptimalOrderExecution called with order type: {orderType} and order amount: {orderAmountInBTC}");
    }

    IList<Order> orderExecutions = new List<Order>();

    List<OrderBook> orderBooks = (await _orderManager.GetOrderBooks(ORDER_BOOK_DEPTH, cancellationToken)).ToList();

    List<OrderWrapper> orders = orderBooks
      .SelectMany(orderBooks => orderType == OrderTypeEnum.Buy ? orderBooks.Asks : orderBooks.Bids)
      .OrderBy(wrapper => orderType == OrderTypeEnum.Buy ? wrapper.Order.Price : -wrapper.Order.Price)
      .ToList();

    decimal remaingOrderAmount = orderAmountInBTC;

    while (remaingOrderAmount > 0 && orders.Count > 0)
    {
      Order order = orders.First().Order;
      orders.RemoveAt(0);

      OrderBook orderBook = orderBooks.Where(book => book.Id == order.OrderBookId).First();

      decimal transactionBTCAmount = order.Amount;

      if (order.Amount > remaingOrderAmount)
      {
        // Partial order fill
        order.Amount = remaingOrderAmount;
        transactionBTCAmount = remaingOrderAmount;
      }

      CheckExchangeBalanceAndAdjustOrder(orderBook, order, transactionBTCAmount, orderType);
      remaingOrderAmount -= order.Amount;

      if (order.Amount != 0)
      {
        orderExecutions.Add(order);
      }

      AdjustExchangeBalance(orderBook, order, transactionBTCAmount, orderType);

      if (remaingOrderAmount != 0 && orders.Count == 0)
      {
        _logger.LogError("There are not enough placed asks/bids to completely fill your order.");
        throw new Exception("There are not enough placed asks/bids to completely fill your order.");
      }
    }

    if (_logger.IsEnabled(LogLevel.Debug))
    {
      _logger.LogDebug($"GetOptimalOrderExecution finished with order executions: {orderExecutions}");
    }

    return orderExecutions;
  }

  private void AdjustExchangeBalance(OrderBook orderBook, Order order, decimal BTCAmount, OrderTypeEnum orderType)
  {
    if (_logger.IsEnabled(LogLevel.Debug))
    {
      _logger.LogDebug($"Adjusting exchange balance for order type: {orderType} and amount: {BTCAmount}");
    }

    if (orderType == OrderTypeEnum.Buy)
    {
      orderBook.BalanceBTC = orderBook.BalanceBTC + BTCAmount;
      orderBook.BalanceEUR = orderBook.BalanceEUR - BTCAmount * order.Price;
    }
    else if (orderType == OrderTypeEnum.Sell)
    {
      orderBook.BalanceBTC = orderBook.BalanceBTC - BTCAmount;
      orderBook.BalanceEUR = orderBook.BalanceEUR + BTCAmount * order.Price;
    }
  }

  private void CheckExchangeBalanceAndAdjustOrder(OrderBook orderBook, Order order, decimal BTCAmount, OrderTypeEnum orderType)
  {
    if (_logger.IsEnabled(LogLevel.Debug))
    {
      _logger.LogDebug($"Checking exchange balance for order type: {orderType} and amount: {BTCAmount}");
    }

    if (orderType == OrderTypeEnum.Buy)
    {
      if (orderBook.BalanceEUR < BTCAmount * order.Price)
      {
        order.Amount = orderBook.BalanceEUR / order.Price;

        _logger.LogInformation("Insufficient exchange EUR balance to fill the whole order. Partial filling order.");
      }
    }
    else if (orderType == OrderTypeEnum.Sell)
    {
      if (orderBook.BalanceBTC < BTCAmount)
      {
        order.Amount = orderBook.BalanceBTC;

        _logger.LogInformation("Insufficient exchange BTC balance to fill the whole order. Partial filling order.");
      }
    }
  }
}
