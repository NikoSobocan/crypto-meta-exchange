using MetaExchange.Infrastructure;
using OrderManagement.Impl;
using OrderManagement.Interfaces;
using OrderManagement.Interfaces.IDataProviders;
using OrderService.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddConsole();

builder.Services.AddSingleton<IDataProvider, DataProvider>();
builder.Services.AddSingleton<IOrderManager, OrderManager>();
builder.Services.AddSingleton<IOrderService, OrderService.Impl.Services.OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
