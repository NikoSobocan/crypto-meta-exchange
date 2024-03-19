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

    #region GM
    [TestMethod]
    [DataRow(OrderTypeEnum.Buy, 0.3, 220)]
    [DataRow(OrderTypeEnum.Sell, 0.4, 2000.10)]
    public async Task Test_GM(OrderTypeEnum orderType, double units, double totalEUR)
    {
      decimal unitsM = (decimal)units;
      decimal totalEURM = (decimal)totalEUR;

      var orderBooks = GetGMOrderBooks();

      _orderManagerMock
        .Setup(om => om.GetOrderBooks(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(orderBooks);

      var orderExecutionPlan = await _orderService.GetOptimalOrderExecution(orderType, unitsM, CancellationToken.None);

      orderExecutionPlan.Should().NotBeNull();

      decimal boughtOrSoldUnits = orderExecutionPlan.Sum(o => o.Amount);
      boughtOrSoldUnits.Should().Be(unitsM);

      decimal spentOrReceivedEur = orderExecutionPlan.Sum(o => o.Amount * o.Price);
      spentOrReceivedEur.Should().Be(totalEURM);
    }

    private static List<OrderBook> GetGMOrderBooks()
    {
      var expensiveOrderBook = new OrderBook
      {
        Id = 1,
        BalanceBTC = 0.15m,
        BalanceEUR = 9000m
      };
      expensiveOrderBook.Bids = new List<OrderWrapper>
      {
        new()
        {
          Order = new()
          {
            OrderBookId = expensiveOrderBook.Id,
            Type = OrderTypeEnum.Buy,
            Amount = 0.1m,
            Price = 10000m
          }
        },
        new()
        {
          Order = new()
          {
            OrderBookId = expensiveOrderBook.Id,
            Type = OrderTypeEnum.Buy,
            Amount = 0.1m,
            Price = 10000m
          }
        }
      };
      expensiveOrderBook.Asks = new List<OrderWrapper>
      {
        new()
        {
          Order = new()
          {
            OrderBookId = expensiveOrderBook.Id,
            Type = OrderTypeEnum.Sell,
            Amount = 0.2m,
            Price = 2000m
          }
        },
        new()
        {
          Order = new()
          {
            OrderBookId = expensiveOrderBook.Id,
            Type = OrderTypeEnum.Sell,
            Amount = 100m,
            Price = 10000m
          }
        }
      };


      var cheapOrderBook = new OrderBook
      {
        Id = 2,
        BalanceBTC = 9000m,
        BalanceEUR = 15m
      };
      cheapOrderBook.Bids = new List<OrderWrapper>
      {
        new()
        {
          Order = new()
          {
            OrderBookId = cheapOrderBook.Id,
            Type = OrderTypeEnum.Buy,
            Amount = 0.2m,
            Price = 0.5m
          }
        },
        new()
        {
          Order = new()
          {
            OrderBookId = cheapOrderBook.Id,
            Type = OrderTypeEnum.Buy,
            Amount = 100m,
            Price = 0.1m
          }
        }
      };
      cheapOrderBook.Asks = new List<OrderWrapper>
      {
        new()
        {
          Order = new()
          {
            OrderBookId = cheapOrderBook.Id,
            Type = OrderTypeEnum.Sell,
            Amount = 0.1m,
            Price = 100m
          }
        },
        new()
        {
          Order = new()
          {
            OrderBookId = cheapOrderBook.Id,
            Type = OrderTypeEnum.Sell,
            Amount = 0.1m,
            Price = 100m
          }
        }
      };

      return new List<OrderBook>
      {
        expensiveOrderBook,
        cheapOrderBook
      };
    }
    #endregion GM

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