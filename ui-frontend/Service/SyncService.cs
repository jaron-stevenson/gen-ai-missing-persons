using System.Text.Json;
using ui_frontend.Models;



namespace ui_frontend.Service
{
    public class SyncService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SyncService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<bool> SyncTrip(Trip trip)
        {
            var client = _httpClientFactory.CreateClient("SyncAPI");
            var response = await client.PostAsJsonAsync("travel-data", trip);
            Console.WriteLine(response.Content.ToString());
            return response.IsSuccessStatusCode;
        }
    }
}
