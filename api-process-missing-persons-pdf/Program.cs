using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Logging;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

string ApiDeploymentName = Environment.GetEnvironmentVariable("ApiDeploymentName", EnvironmentVariableTarget.Process) ?? "";
string ApiEndpoint = Environment.GetEnvironmentVariable("ApiEndpoint", EnvironmentVariableTarget.Process) ?? "";
string ApiKey = Environment.GetEnvironmentVariable("ApiKey", EnvironmentVariableTarget.Process) ?? "";
string AppInsightsConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")?? "";

// Not being used but might be needed in the future
// string TextEmbeddingName = Environment.GetEnvironmentVariable("EmbeddingName", EnvironmentVariableTarget.Process) ?? "";
// string BingSearchEndPoint = Environment.GetEnvironmentVariable("BingSearchApiEndPoint", EnvironmentVariableTarget.Process) ?? "";
// string BingSearchKey = Environment.GetEnvironmentVariable("BingSearchKey", EnvironmentVariableTarget.Process) ?? "";

// Let's add some OpenTelemetry to the mix so we can listen to SPANs and METRICS from Semantic Kernel
// Here is a great link that covers this.  https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/telemetry-with-console

// Enable mnodel diagnostics with sensitive data.
AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

var connectionString = AppInsightsConnectionString;
// Using resource builder to add service name to all telemetry items
var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService("TelemetryMyExample");
// Create the OpenTelemetry TracerProvider and MeterProvider
using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Microsoft.SemanticKernel*")
    .AddSource("TelemetryMyExample")
    .AddAzureMonitorTraceExporter(options => options.ConnectionString = connectionString)
    .Build();

using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("Microsoft.SemanticKernel*")
    .AddAzureMonitorMetricExporter(options => options.ConnectionString = connectionString)
    .Build();
// Create the OpenTelemetry LoggerFactory
using var loggerFactory = LoggerFactory.Create(builder =>
{
    // Add OpenTelemetry as a logging provider
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(resourceBuilder);
        options.AddAzureMonitorLogExporter(options => options.ConnectionString = connectionString);
        // Format log messages. This is default to false.
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Information);   
});
// Now, we can see telemetry from Semantic Kernel in Azure Monitor and Application Insights
// Anywhere the Semantic Kernel is used in the application, telemetry will be sent to Azure Monitor and Application Insights

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        // services.AddApplicationInsightsTelemetryWorkerService();
        // services.ConfigureFunctionsApplicationInsights();

        services.AddTransient<Kernel>(s =>
        {
            var builder = Kernel.CreateBuilder();
            builder.Services.AddSingleton(loggerFactory);
            builder.AddAzureOpenAIChatCompletion(
                ApiDeploymentName,
                ApiEndpoint,
                ApiKey
                );

            return builder.Build();
        });
        
        services.AddSingleton<IChatCompletionService>(sp =>
                     sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());

        // // This will need to change to a ChatHistoryManager to manage chat histories based on client ID or persist and retrieve chat history from a database
        // // For not using this just to get something up and running
        services.AddSingleton<ChatHistory>(s =>
        {
           var chathistory = new ChatHistory();
           return chathistory;
        });

        // The following is not being used but could come in handy in the future
        /*
        // Add the ChatHistoryManager as a singleton service to manage chat histories based on client ID
        services.AddSingleton<IChatHistoryManager>(sp =>
        {
            string systemmsg = CorePrompts.GetSystemPrompt();
            return new ChatHistoryManager(systemmsg);
        });

        // AddHostedService - ASP.NET will run the ChatHistoryCleanupService in the background and will clean up all chathistores that are older than 1 hour
        services.AddHostedService<ChatHistoryCleanupService>();

        services.AddHttpClient<IBingSearchClient, BingSearchClient>(client =>
        {
            client.BaseAddress = new Uri(BingSearchEndPoint);
        });

        services.AddSingleton<IBingSearchClient>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(IBingSearchClient));
            var apiKey = BingSearchKey;
            var endpoint = BingSearchEndPoint;
            return new BingSearchClient(httpClient, apiKey, endpoint);
        });
        */
    })
    .Build();

host.Run();
