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
    .ConfigureServices((context, services) =>
    {
        services.AddAutoMapper(typeof(MappingProfile));

        // Use the connection string directly as a string, without Uri wrapping
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? throw new InvalidOperationException("AzureWebJobsStorage environment variable is not set.");
        var tableStorageReviewsTableName = Environment.GetEnvironmentVariable("TableStorageReviewsTableName") ?? throw new InvalidOperationException("TableStorageReviewsTableName environment variable is not set.");

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Setup the table client using the connection string and table name
        services.AddSingleton(sp => new TableClient(connectionString, tableStorageReviewsTableName));

        services.AddDbContext<WidgetStoreDbContext>();
        services.AddSingleton<IReviewRepository, ReviewRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IReviewService, ReviewService>();
        services.AddSingleton<IUserService, UserService>();
    })
    .Build();

host.Run();