
using OrderManagement.Interfaces.Responses;

namespace OrderManagement.Interfaces.IDataProviders;

public interface IDataProvider
{
  Task<IList<OrderBook>> GetOrderBookData(int numberOfBooks);
}
