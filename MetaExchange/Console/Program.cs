using MetaExchange.Infrastructure;

Console.WriteLine("Hello, World!");

DataProvider provider = new DataProvider();

var temp = await provider.GetOrderBookData(3);

Console.ReadKey();