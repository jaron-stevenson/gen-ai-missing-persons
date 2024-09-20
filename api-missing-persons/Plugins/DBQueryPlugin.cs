namespace api_missing_persons.Plugins
{
    using Microsoft.SemanticKernel;
    using System.ComponentModel;
    using api_missing_persons.Interfaces;

    public class DBQueryPlugin(IAzureDbService azureDbService)
    {
        [KernelFunction]
        [Description("Executes a SQL query to get a data for missing persons.")]
        public async Task<string> GetMissingPersons(string query)
        {
            var dbResults = await azureDbService.GetDbResults(query);
            return dbResults;
        }
    }
}
