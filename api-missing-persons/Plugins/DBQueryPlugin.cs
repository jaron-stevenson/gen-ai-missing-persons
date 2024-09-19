namespace api_missing_persons.Plugins
{
    using Microsoft.SemanticKernel;
    using System.ComponentModel;
    using api_missing_persons.Services;

    public class DBQueryPlugin
    {
        private string _dbConnection;

        public DBQueryPlugin(IConfiguration configuration)
        {
            _dbConnection = configuration.GetValue<string>("DatabaseConnection");
        }

        [KernelFunction]
        [Description("Executes a SQL query to get a list of missing persons.")]
        public async Task<string> GetMissingPersons(string query)
        {
            var azureDbService = new AzureDbService(_dbConnection);
            var dbResults = await azureDbService.GetDbResults(query);

            string results = dbResults;

            return results;
        }
    }
}
