using MetaExchange.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Impl;
using OrderManagement.Interfaces;
using OrderManagement.Interfaces.IDataProviders;
using OrderService.Interfaces.Services;

namespace MetaExchange.Core.ServiceDI;

public static class ServiceDI
{
  public static IServiceCollection AddServices(this IServiceCollection services)
  {
    services.AddSingleton<IDataProvider, DataProvider>();
    services.AddSingleton<IOrderManager, OrderManager>();
    services.AddSingleton<IOrderService, OrderService.Impl.Services.OrderService>();

    return services;
  }
}
