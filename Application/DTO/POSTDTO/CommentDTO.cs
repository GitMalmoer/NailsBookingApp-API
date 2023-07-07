using System.ComponentModel.DataAnnotations;

namespace Application.DTO.POSTDTO
{
    public class CommentDTO
    {
        [Required] public int PostId { get; set; }

        [Required] public string ApplicationUserId { get; set; }

        [Required] public string commentContent { get; set; }

    }
}
