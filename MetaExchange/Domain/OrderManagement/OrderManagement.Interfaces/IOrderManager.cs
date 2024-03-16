using OrderManagement.Interfaces.Responses;

namespace OrderManagement.Interfaces;

public interface IOrderManager
{
  Task<IList<OrderBook>> GetOrderBooks(int numberOfBooks, CancellationToken cancellationToken);
}

