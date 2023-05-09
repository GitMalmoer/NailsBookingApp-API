using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO.AUTHDTO
{
    public class ConfirmEmailDTO
    {
        [Required]public string userId { get; set; }
        [Required]public string token { get; set; }
    }
}
