namespace api_missing_persons.Interfaces
{
    public interface IAzureDbService
    {
        Task<string> GetDbResults(string query);
    }
}
