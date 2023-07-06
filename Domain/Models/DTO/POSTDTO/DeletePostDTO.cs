using System.ComponentModel.DataAnnotations;

namespace Domain.Models.DTO.POSTDTO
{
    public class DeletePostDTO
    {
        [Required] public string ApplicationUserId { get; set; }
        [Required] public int PostId { get; set; }
    }
}
