using Newtonsoft.Json;
using OrderManagement.Interfaces;
using OrderManagement.Interfaces.IDataProviders;
using System.Reflection;

namespace MetaExchange.Infrastructure;

public class DataProvider : IDataProvider
{
  private const string DATA_FILE_PATH = "../../../../Infrastructure/OrderManagement/Data/order_books_data";
  string filePath = Environment.GetEnvironmentVariable("DATA_FILE_PATH") ?? DATA_FILE_PATH;

  public async Task<IList<OrderBook>> GetOrderBookData(int numberOfBooks)
  {
    var orderBooks = new List<OrderBook>();

    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
    using (var reader = new StreamReader(fileStream))
    {
      int booksCounter = 0;
      string? line;
      string exchangeBTCBalance;
      string exchangeEURBalance;
      while ((line = reader.ReadLine()) != null)
      {
        exchangeEURBalance = line.Substring(0, line.IndexOf('.'));
        exchangeBTCBalance = line.Substring(line.IndexOf('.') + 1, line.IndexOf('\t') - line.IndexOf('.') - 1);

        line = line.Substring(line.IndexOf('\t') + 1);
        OrderBook? orderBook = JsonConvert.DeserializeObject<OrderBook>(line);

        if (orderBook != null)
        {
          orderBook.Id = booksCounter + 1;

          if (decimal.TryParse(exchangeEURBalance, out decimal parsedEURBalance))
          {
            orderBook.BalanceEUR = parsedEURBalance;
          };
          if (decimal.TryParse(exchangeBTCBalance, out decimal parsedBTCBalance))
          {
            orderBook.BalanceBTC = parsedBTCBalance;
          };

          orderBook.Bids.ToList().ForEach(orderWrapper => orderWrapper.Order.OrderBookId = orderBook.Id);
          orderBook.Asks.ToList().ForEach(orderWrapper => orderWrapper.Order.OrderBookId = orderBook.Id);

          orderBooks.Add(orderBook);
          booksCounter++;

          if (booksCounter == numberOfBooks)
          {
            break;
          }
        }
      }
    }

    return orderBooks;
  }
}
