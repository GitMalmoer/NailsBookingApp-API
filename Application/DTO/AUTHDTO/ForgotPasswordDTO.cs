using System.ComponentModel.DataAnnotations;

namespace Application.DTO.AUTHDTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
