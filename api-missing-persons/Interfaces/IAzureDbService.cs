using api_missing_persons.Models;

namespace api_missing_persons.Interfaces
{
    public interface IAzureDbService
    {
        Task<string> GetDbResults(string query);
        Task<PersonDetail> GetMissingPerson(string name, int age, DateTime dateReported);
        Task<int> UpdateMissingPerson(int id, DateTime dateFound);
    }
}
