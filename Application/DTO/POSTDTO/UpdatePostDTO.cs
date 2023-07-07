using System.ComponentModel.DataAnnotations;

namespace Application.DTO.POSTDTO
{
    public class UpdatePostDTO
    {
        [Required] public int PostId { get; set;}
        [Required] public string ApplicationUserId { get; set; }
        [Required] public string Content { get; set; }
    }
}
