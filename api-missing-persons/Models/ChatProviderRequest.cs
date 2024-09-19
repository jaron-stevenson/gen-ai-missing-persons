using System.ComponentModel.DataAnnotations;

namespace api_missing_persons.Models
{
    public class ChatProviderRequest
    {
        public string? SessionId { get; set; }
        [Required]
        public string Prompt { get; set; }
        public string? ChatName { get; set; }
    }
}
