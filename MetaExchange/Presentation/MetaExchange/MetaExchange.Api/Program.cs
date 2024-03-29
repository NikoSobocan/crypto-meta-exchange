using MetaExchange.Core.Filters;
using MetaExchange.Infrastructure;
using OrderManagement.Impl;
using OrderManagement.Interfaces;
using OrderManagement.Interfaces.IDataProviders;
using OrderService.Interfaces.Services;
using System.Text.Json.Serialization;
using MetaExchange.Core.ServiceDI;


var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddControllers(options =>
  {
    options.Filters.Add<CustomExceptionFilter>();
  })
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
  });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddConsole();

builder.Services.AddServices();

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
