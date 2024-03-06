namespace OrderManagement.Interfaces;

public class OrderBook
{
  public string AcqTime { get; set; } = default!;
  public IList<OrderWrapper> Bids { get; set; } = default!;
  public IList<OrderWrapper> Asks { get; set; } = default!;
}
