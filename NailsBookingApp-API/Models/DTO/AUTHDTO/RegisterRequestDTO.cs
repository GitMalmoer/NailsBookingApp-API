using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO.AUTHDTO
{
    public class RegisterRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [Compare("Password", ErrorMessage = "The password and confirm password do not match")]
        public string ConfirmPassword { get; set; }



    }
}
