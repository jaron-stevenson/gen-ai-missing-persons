using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

string ApiDeploymentName = Environment.GetEnvironmentVariable("ApiDeploymentName", EnvironmentVariableTarget.Process) ?? "";
string ApiEndpoint = Environment.GetEnvironmentVariable("ApiEndpoint", EnvironmentVariableTarget.Process) ?? "";
string ApiKey = Environment.GetEnvironmentVariable("ApiKey", EnvironmentVariableTarget.Process) ?? "";
string AppInsightsConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")?? "";

// Not being used but might be needed in the future
// string TextEmbeddingName = Environment.GetEnvironmentVariable("EmbeddingName", EnvironmentVariableTarget.Process) ?? "";
// string BingSearchEndPoint = Environment.GetEnvironmentVariable("BingSearchApiEndPoint", EnvironmentVariableTarget.Process) ?? "";
// string BingSearchKey = Environment.GetEnvironmentVariable("BingSearchKey", EnvironmentVariableTarget.Process) ?? "";

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        // services.AddApplicationInsightsTelemetryWorkerService();
        // services.ConfigureFunctionsApplicationInsights();

        services.AddTransient<Kernel>(s =>
        {
            var builder = Kernel.CreateBuilder();
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
