
using OrderManagement.Interfaces.Responses;
using System.Threading;

namespace OrderManagement.Interfaces.IDataProviders;

public interface IDataProvider
{
  Task<IList<OrderBook>> GetOrderBookData(int numberOfBooks, CancellationToken cancellationToken);
}
