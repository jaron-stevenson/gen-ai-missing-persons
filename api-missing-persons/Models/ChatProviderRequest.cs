using System.ComponentModel.DataAnnotations;

namespace api_missing_persons.Models
{
    public class ChatProviderRequest
    {
        [Required]
        public string Prompt { get; set; }
        public string? ChatName { get; set; }
    }
}
