using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WidgetAndCo.Business;
using WidgetAndCo.Core.Interfaces;
using WidgetAndCo.Core.Mapper;
using WidgetAndCo.Data;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? throw new InvalidOperationException("AzureWebJobsStorage environment variable is not set.");
        var tableStorageOrderTableName = Environment.GetEnvironmentVariable("TableStorageOrderTableName") ?? throw new InvalidOperationException("TableStorageOrderTableName environment variable is not set.");
        var tableStorageOrderProductTableName = Environment.GetEnvironmentVariable("TableStorageOrderProductTableName") ?? throw new InvalidOperationException("TableStorageOrderProductTableName environment variable is not set.");


        // Create two table clients, one for each table
        services.AddSingleton<IOrderRepository>(sp => new OrderRepository(new TableClient(connectionString,
                tableStorageOrderTableName),
            new TableClient(connectionString,
                tableStorageOrderProductTableName)));

        services.AddDbContext<WidgetStoreDbContext>();
        services.AddSingleton<IProductRepository, ProductRepository>();
        services.AddSingleton<IOrderService, OrderService>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserService, UserService>();
    })
    .Build();

host.Run();