namespace OrderManagement.Interfaces;

public class Order
{
  public int? Id { get; set; }
  public DateTime Time { get; set; }
  public OrderTypeEnum Type { get; set; }
  public OrderKindEnum Kind { get; set; }
  public decimal Amount { get; set; }
  public decimal Price { get; set; }
}
