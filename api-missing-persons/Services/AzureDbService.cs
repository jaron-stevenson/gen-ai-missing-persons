namespace api_missing_persons.Services
{
    using api_missing_persons.Interfaces;
    using Dapper;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text.Json;

    public class AzureDbService : IAzureDbService
    {
        private readonly string _connectionString;

        public AzureDbService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> GetDbResults(string query)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            var dbResult = await connection.QueryAsync<dynamic>(query);
            var jsonString = JsonSerializer.Serialize(dbResult);

            return jsonString;
        }
    }    
}
