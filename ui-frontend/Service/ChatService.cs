using System.Text.Json;
using ui_frontend.Models;



namespace ui_frontend.Service
{
    public class ChatService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatService(IHttpClientFactory httpClientFactory)
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

        public async Task<TourGuideResponse> GetAiResponseAsync(string query, SessionInfo sessioninfo)
        {
            var client = _httpClientFactory.CreateClient("ChatAPI");
            var response = await client.PostAsJsonAsync("ChatProvider", sessioninfo);
            TourGuideResponse? chatResponse = null;
            string? responseBody = null;
            if (response.IsSuccessStatusCode)
            {
                // Read the content of the response as a string
                responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    chatResponse = JsonSerializer.Deserialize<TourGuideResponse>(responseBody ?? "", GetOptions());
                }
                catch (JsonException)
                {
                    // Failed to parse as JSON, treat as plain string
                }
            }
            if (chatResponse == null)
            {
                chatResponse = new TourGuideResponse { Message = responseBody };
            }
            Console.WriteLine(responseBody);

            return chatResponse;
        }
    }
}
