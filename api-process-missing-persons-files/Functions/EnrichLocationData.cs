using api_process_missing_persons_files.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace api_process_missing_persons_files.Functions
{
    public class EnrichLocationData
    {
        // private readonly ILogger<EnrichLocationData> _logger;
        private readonly ILogger _logger;
        private readonly AddressEnrichmentHelper _enrichmentHelper;

        //public EnrichLocationData(ILogger<EnrichLocationData> logger)
        //{
        //    _logger = logger;
        //}

        public EnrichLocationData(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EnrichLocationData>();
            _enrichmentHelper = new AddressEnrichmentHelper(_logger, httpClientFactory, connectionString, mapsApiKey);
        }

        [Function("EnrichData")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
