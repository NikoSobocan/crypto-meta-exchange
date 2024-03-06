
namespace OrderManagement.Interfaces.IDataProviders;

public interface IDataProvider
{
  Task<IList<OrderBook>> GetOrderBookData();
}
