using System.ComponentModel.DataAnnotations;

namespace Domain.Models.DTO.AUTHDTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
