using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OrderManagement.Interfaces.Enums;
using OrderService.Interfaces.Services;

namespace Presentation.Console;

public class HostedService : IHostedService
{
  private readonly IOrderService _orderService;
  private readonly ILogger<HostedService> _logger;

  public HostedService(IOrderService orderService, ILogger<HostedService> logger)
  {
    _orderService = orderService;
    _logger = logger;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("HostedService is starting.");

    try
    {
      await DoWorkAsync(cancellationToken);
    }
    catch (Exception ex)
    {
      _logger.LogError($"An unhandled exception occurred: {ex}");
    }
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("HostedService is stopping.");
    return Task.CompletedTask;
  }

  private async Task DoWorkAsync(CancellationToken cancellationToken)
  {
    while (!cancellationToken.IsCancellationRequested)
    {
      System.Console.WriteLine("Please enter type of order (Buy/Sell):");
      Enum.TryParse(System.Console.ReadLine(), out OrderTypeEnum orderType);

      System.Console.WriteLine("Please enter order amount:");
      decimal.TryParse(System.Console.ReadLine(), out decimal orderAmount);

      foreach (var order in await _orderService.GetOptimalOrderExecution(orderType, orderAmount, cancellationToken))
      {
        System.Console.WriteLine('\n' + JsonConvert.SerializeObject(order, new StringEnumConverter()));
      }
    }
  }
}