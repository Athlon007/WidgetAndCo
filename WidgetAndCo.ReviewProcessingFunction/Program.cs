using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Use the connection string directly as a string, without Uri wrapping
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? throw new InvalidOperationException("AzureWebJobsStorage environment variable is not set.");
        var tableStorageReviewsTableName = Environment.GetEnvironmentVariable("TableStorageReviewsTableName") ?? throw new InvalidOperationException("TableStorageReviewsTableName environment variable is not set.");

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Setup the table client using the connection string and table name
        services.AddSingleton(sp => new TableClient(connectionString, tableStorageReviewsTableName));
    })
    .Build();

host.Run();