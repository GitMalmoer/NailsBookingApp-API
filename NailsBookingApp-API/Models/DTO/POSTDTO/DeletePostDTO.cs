using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO.POSTDTO
{
    public class DeletePostDTO
    {
        [Required] public string ApplicationUserId { get; set; }
        [Required] public int PostId { get; set; }
    }
}
