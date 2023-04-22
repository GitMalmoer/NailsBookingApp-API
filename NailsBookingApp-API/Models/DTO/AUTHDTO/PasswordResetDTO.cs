using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO.AUTHDTO
{
    public class PasswordResetDTO
    {
        [Required] public string Password { get; set; }

        [Compare("Password",ErrorMessage = "Passwords do not match")]
        [Required] public string ConfirmPassword { get; set; }

        public string token { get; set; }
        public string email { get; set; }

    }
}
