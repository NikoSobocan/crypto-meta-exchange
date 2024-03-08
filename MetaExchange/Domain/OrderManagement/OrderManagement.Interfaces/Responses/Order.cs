using OrderManagement.Interfaces.Enums;

namespace OrderManagement.Interfaces.Responses;

public class Order
{
    public int? Id { get; set; }
    public int OrderBookId { get; set; }
    public DateTime Time { get; set; }
    public OrderTypeEnum Type { get; set; }
    public OrderKindEnum Kind { get; set; }
    public decimal Amount { get; set; }
    public decimal Price { get; set; }
}
