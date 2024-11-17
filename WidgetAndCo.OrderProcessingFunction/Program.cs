using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WidgetAndCo.Business;
using WidgetAndCo.Core.Interfaces;
using WidgetAndCo.Core.Mapper;
using WidgetAndCo.Data;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddDbContext<WidgetStoreDbContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("DefaultConnection") ?? throw new InvalidOperationException("AzureWebJobsStorage environment variable is not set."));
});


builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IOrderProductRepository, OrderProductRepository>();

builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IOrderProductRepository, OrderProductRepository>();

builder.Services.AddSingleton<IProductRepository, ProductRepository>();

builder.Services.AddDbContext<WidgetStoreDbContext>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IProductService, ProductService>();

builder.Build().Run();