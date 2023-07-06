using System.ComponentModel.DataAnnotations;

namespace Application.DTO.AUTHDTO
{
    public class ConfirmEmailDTO
    {
        [Required]public string userId { get; set; }
        [Required]public string token { get; set; }
    }
}
