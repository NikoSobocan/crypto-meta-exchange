using OrderManagement.Interfaces;
using OrderManagement.Interfaces.IDataProviders;
using OrderManagement.Interfaces.Responses;

namespace OrderManagement.Impl;

public class OrderManager : IOrderManager
{
  private readonly IDataProvider _dataProvider;

  public OrderManager(IDataProvider dataProvider)
  {
    _dataProvider = dataProvider;
  }

  public async Task<IList<OrderBook>> GetOrderBooks(int numberOfBooks)
  {
    return await _dataProvider.GetOrderBookData(numberOfBooks);
  }
}
