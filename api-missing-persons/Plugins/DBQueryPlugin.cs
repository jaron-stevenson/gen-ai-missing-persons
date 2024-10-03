namespace api_missing_persons.Plugins
{
    using Microsoft.SemanticKernel;
    using System.ComponentModel;
    using api_missing_persons.Interfaces;

    public class DBQueryPlugin(IAzureDbService azureDbService)
    {
        [KernelFunction]
        [Description("Executes a SQL query to get data for missing persons. Attempt to query name column if not enough information in prompt.")]
        public async Task<string> GetMissingPersons(string query)
        {
            var dbResults = await azureDbService.GetDbResults(query);
            return dbResults;
        }
    }
}
