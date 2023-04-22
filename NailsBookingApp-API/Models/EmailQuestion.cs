using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models
{
    public class EmailQuestion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
