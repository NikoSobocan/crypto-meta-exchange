namespace OrderManagement.Interfaces.Responses;

public class OrderBook
{
    public int Id { get; set; }
    public decimal BalanceEUR { get; set; } = default!;
    public decimal BalanceBTC { get; set; } = default;
    public string AcqTime { get; set; } = default!;
    public IList<OrderWrapper> Bids { get; set; } = default!;
    public IList<OrderWrapper> Asks { get; set; } = default!;
}
