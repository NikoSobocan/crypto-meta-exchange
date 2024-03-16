using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using OrderManagement.Interfaces;
using OrderManagement.Interfaces.Enums;
using OrderManagement.Interfaces.Responses;
using OrderService.Interfaces.Services;

namespace OrderService.Tests.Services
{
  [TestClass]
  public class OrderServiceTests
  {
    private IFixture? _fixture;

    private IOrderService _orderService = null!;
    private Mock<IOrderManager> _orderManagerMock = null!;

    [TestInitialize]
    public void InitTest()
    {
      _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
      _orderManagerMock = _fixture.Freeze<Mock<IOrderManager>>();
      _orderService = _fixture.Create<Impl.Services.OrderService>();
    }

    [TestMethod]
    public async Task GetOptimalOrderExecution_Should_ReturnOrderExecution()
    {
      List<OrderBook> orderBooks = GetMockOrderBookData();

      _orderManagerMock.Setup(x => x.GetOrderBooks(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(orderBooks);

      IList<Order> orderExecution = await _orderService.GetOptimalOrderExecution(OrderTypeEnum.Buy, 1.5M, CancellationToken.None);

      _orderManagerMock.Verify(x => x.GetOrderBooks(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
      orderExecution.Count.Should().Be(2);
    }

    [TestMethod]
    public async Task GetOptimalOrderExecution_InsufficientExchangeBTCBalance_Should_ThrowException()
    {
      List<OrderBook> orderBooks = GetMockOrderBookData();

      _orderManagerMock.Setup(x => x.GetOrderBooks(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(orderBooks);

      await Assert.ThrowsExceptionAsync<Exception>(async () =>
      {
        IList<Order> orderExecution = await _orderService.GetOptimalOrderExecution(OrderTypeEnum.Buy, 1000, CancellationToken.None);
      });
    }

    [TestMethod]
    public async Task GetOptimalOrderExecution_InsufficientExchangeEURBalance_Should_ThrowException()
    {
      List<OrderBook> orderBooks = GetMockOrderBookData();

      _orderManagerMock.Setup(x => x.GetOrderBooks(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(orderBooks);

      await Assert.ThrowsExceptionAsync<Exception>(async () =>
      {
        IList<Order> orderExecution = await _orderService.GetOptimalOrderExecution(OrderTypeEnum.Sell, 1000, CancellationToken.None);
      });
    }

    [TestMethod]
    public async Task GetOptimalOrderExecution_BuyOrder_Should_AdjustExchangeBalance()
    {
      List<OrderBook> orderBooks = GetMockOrderBookData();

      List<OrderBook> orderBooksCopy = GetMockOrderBookData();

      _orderManagerMock.Setup(x => x.GetOrderBooks(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(orderBooks);

      IList<Order> orderExecution = await _orderService.GetOptimalOrderExecution(OrderTypeEnum.Buy, 1, CancellationToken.None);

      orderBooks[0].BalanceBTC.Should().BeLessThan(orderBooksCopy[0].BalanceBTC);
      orderBooks[0].BalanceEUR.Should().BeGreaterThan(orderBooksCopy[0].BalanceEUR);

    }

    [TestMethod]
    public async Task GetOptimalOrderExecution_SellOrder_Should_AdjustExchangeBalance()
    {
      List<OrderBook> orderBooks = GetMockOrderBookData();

      List<OrderBook> orderBooksCopy = GetMockOrderBookData();

      _orderManagerMock.Setup(x => x.GetOrderBooks(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(orderBooks);

      IList<Order> orderExecution = await _orderService.GetOptimalOrderExecution(OrderTypeEnum.Sell, 1, CancellationToken.None);

      orderBooks[0].BalanceBTC.Should().BeGreaterThan(orderBooksCopy[0].BalanceBTC);
      orderBooks[0].BalanceEUR.Should().BeLessThan(orderBooksCopy[0].BalanceEUR);
    }

    [TestMethod]
    public async Task GetOptimalOrderExecution_NotEnoughOrdersInBook_Should_ThrowException()
    {
      List<OrderBook> orderBooks = GetMockOrderBookData();

      _orderManagerMock.Setup(x => x.GetOrderBooks(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(orderBooks);

      await Assert.ThrowsExceptionAsync<Exception>(async () =>
      {
        IList<Order> orderExecution = await _orderService.GetOptimalOrderExecution(OrderTypeEnum.Buy, 11, CancellationToken.None);
      });

    }

    private List<OrderBook> GetMockOrderBookData()
    {
      return new List<OrderBook> {
        new OrderBook {
          Id = 1,
          BalanceBTC = 100,
          BalanceEUR = 100000,
          Asks = new List <OrderWrapper> {
            new OrderWrapper {
              Order = new Order {
                OrderBookId = 1,
                Amount = 1,
                Price = 1000,
                Type = OrderTypeEnum.Sell
              }
            },
            new OrderWrapper {
              Order = new Order {
                OrderBookId = 1,
                Amount = 2,
                Price = 1100,
                Type = OrderTypeEnum.Sell
              }
            }
          },
          Bids = new List <OrderWrapper> {
            new OrderWrapper {
                Order = new Order {
                OrderBookId = 1,
                Amount = 1,
                Price = 1500,
                Type = OrderTypeEnum.Buy
              }
            },
            new OrderWrapper {
                Order = new Order {
                OrderBookId = 1,
                Amount = 2,
                Price = 1600,
                Type = OrderTypeEnum.Buy
              }
            }
          }
        },
      };
    }
  }
}