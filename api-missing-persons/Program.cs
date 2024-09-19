using api_missing_persons.Interfaces;
using api_missing_persons.Plugins;
using api_missing_persons.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true); // Add this line

var configuration = builder.Configuration;
var apiDeploymentName = configuration.GetValue<string>("AzureOpenAiDeploymentName");
var apiEndpoint = configuration.GetValue<string>("AzureOpenAiEndpoint");
var apiKey = configuration.GetValue<string>("AzureOpenAiKey");
var connectionString = configuration.GetConnectionString("DatabaseConnection");

// Add services to the container.
builder.Services.AddApplicationInsightsTelemetry();
builder.Logging.AddConsole();

builder.Services.AddTransient<IAzureDbService>(s =>
{
    return new AzureDbService(connectionString);
});

builder.Services.AddTransient<Kernel>(s =>
{
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(
        apiDeploymentName,
        apiEndpoint,
        apiKey);

    builder.Plugins.AddFromType<DBQueryPlugin>();

    return builder.Build();
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();