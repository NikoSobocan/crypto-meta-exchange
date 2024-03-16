﻿using MetaExchange.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderManagement.Impl;
using OrderManagement.Interfaces;
using OrderManagement.Interfaces.IDataProviders;
using OrderService.Interfaces.Services;
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

        services.AddSingleton<IOrderManager, OrderManager>();
        services.AddSingleton<IOrderService, OrderService.Impl.Services.OrderService>();
        services.AddSingleton<IDataProvider, DataProvider>();
        services.AddHostedService<HostedService>();
      }).Build();
  }
}