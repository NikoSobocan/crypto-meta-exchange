using Newtonsoft.Json;
using OrderManagement.Interfaces;
using OrderManagement.Interfaces.IDataProviders;

namespace MetaExchange.Infrastructure;

public class DataProvider : IDataProvider
{
  private const string DATA_FILE_PATH = "../../../../Infrastructure/OrderManagement/Data/order_books_data";

  public async Task<IList<OrderBook>> GetOrderBookData(int numberOfBooks)
  {
    var orderBooks = new List<OrderBook>();

    using (var fileStream = new FileStream(DATA_FILE_PATH, FileMode.Open, FileAccess.Read))
    using (var reader = new StreamReader(fileStream))
    {
      int booksCounter = 0;
      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        line = line.Substring(line.IndexOf('\t') + 1);
        OrderBook? orderBook = JsonConvert.DeserializeObject<OrderBook>(line, new JsonSerializerSettings { Culture = System.Globalization.CultureInfo.CurrentCulture });

        if (orderBook != null)
        {
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
