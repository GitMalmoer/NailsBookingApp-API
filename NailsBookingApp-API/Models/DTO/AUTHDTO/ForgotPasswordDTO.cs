using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO.AUTHDTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
