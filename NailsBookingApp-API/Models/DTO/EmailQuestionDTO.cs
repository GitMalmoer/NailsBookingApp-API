using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO
{
    public class EmailQuestionDTO
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Maximum length of name field is 50 letters")]
        public string Name { get; set; }
        [Required]

        [MaxLength(50, ErrorMessage = "Maximum length of email field is 50 letters")]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(250, ErrorMessage = "Maximum length of message field is 250 letters")]
        public string Message { get; set; }
    }
}
