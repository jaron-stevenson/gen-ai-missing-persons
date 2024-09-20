namespace api_missing_persons.Plugins
{
    using Microsoft.SemanticKernel;
    using System.ComponentModel;
    using api_missing_persons.Interfaces;

    public class DBQueryPlugin
    {
        private string _dbConnection;
        private IAzureDbService _azureDbService;

        public DBQueryPlugin(IAzureDbService azureDbService)
        {
            _azureDbService = azureDbService;
        }

        [KernelFunction]
        [Description("Executes a SQL query to get a data for missing persons.")]
        public async Task<string> GetMissingPersons(string query)
        {
            var dbResults = await _azureDbService.GetDbResults(query);
            return dbResults;
        }
    }
}
