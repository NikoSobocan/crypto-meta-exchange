using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderManagement.Interfaces;
using OrderManagement.Interfaces.IDataProviders;
using System.Reflection.Metadata;

namespace MetaExchange.Infrastructure;

public class DataProvider : IDataProvider
{
  private const string dataFilePath = "../../../../Infrastructure/OrderManagement/Data/order_books_data";

  public async Task<IList<OrderBook>> GetOrderBookData()
  {
    var orderBooks = new List<OrderBook>();

    using (var fileStream = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
    using (var reader = new StreamReader(fileStream))
    {
      string? line;
      while ((line = reader.ReadLine()) != null)
      {
        line = line.Substring(line.IndexOf('\t') + 1);
        OrderBook? orderBook = JsonConvert.DeserializeObject<OrderBook>(line, new JsonSerializerSettings { Culture = System.Globalization.CultureInfo.CurrentCulture });

        if (orderBook != null)
        {
          orderBooks.Add(orderBook);
        }
      }
    }

    return orderBooks;
  }
}
