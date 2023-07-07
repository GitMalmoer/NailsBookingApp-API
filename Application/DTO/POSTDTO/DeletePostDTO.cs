using System.ComponentModel.DataAnnotations;

namespace Application.DTO.POSTDTO
{
    public class DeletePostDTO
    {
        [Required] public string ApplicationUserId { get; set; }
        [Required] public int PostId { get; set; }
    }
}
