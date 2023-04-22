using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO
{
    public class EmailQuestionDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]

        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(250)]
        public string Message { get; set; }
    }
}
