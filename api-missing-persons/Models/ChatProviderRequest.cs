using System.ComponentModel.DataAnnotations;

namespace api_missing_persons.Models
{
    public class ChatProviderRequest
    {
        public string? SessionId { get; set; }

        public string? UserId { get; set; }
        [Required]
        public string Prompt { get; set; }
    }
}
