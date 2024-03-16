using MetaExchange.Core.ServiceDI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Presentation.Console;

internal class Program
{
  public static async Task Main(string[] args)
  {
    var host = CreateHostBuilder(args);

    await host.RunAsync();
  }

  public static IHost CreateHostBuilder(string[] args)
  {
    return Host.CreateDefaultBuilder(args)
      .ConfigureServices((context, services) =>
      {
        services.AddLogging(builder =>
        {
          builder.AddConsole();
          builder.AddDebug();
          builder.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddServices();
        services.AddHostedService<HostedService>();
      }).Build();
  }
}