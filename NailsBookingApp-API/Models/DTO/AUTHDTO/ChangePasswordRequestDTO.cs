using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO.AUTHDTO
{
    public class ChangePasswordRequestDTO
    {
        //EITHER REMOVE THIS OR USE THIS IN FRONTEND BY DECODING JWT
        [EmailAddress]
        [Required]
        public string email { get; set; }

        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "The new password and confirm new password do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}
