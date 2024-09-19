namespace api_missing_persons.Plugins
{
    using Microsoft.SemanticKernel;
    using System.ComponentModel;
    using api_missing_persons.Interfaces;

    public class DBQueryPlugin
    {
        private static bool _hrToggleContact;
        private IAzureDbService _azureDbService;
        private ILogger<DBQueryPlugin> _logger;

        public DBQueryPlugin(
            IAzureDbService azureDbService, 
            ILogger<DBQueryPlugin> logger)
        {
            _azureDbService = azureDbService;
            _logger = logger;
        }

        [KernelFunction]
        [Description("")]
        [return: Description("A list of missing persons.")]
        public async Task<string> GetMissingPersons(string query)
        {
            _logger.LogInformation($"SQL Query: {query}");

            var dbResults = await _azureDbService.GetDbResults(query);

            string results = dbResults;

            _logger.LogInformation($"DB Results:{results}");
            return results;
        }
    }
}
