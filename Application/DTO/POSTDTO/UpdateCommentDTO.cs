using System.ComponentModel.DataAnnotations;

namespace Application.DTO.POSTDTO
{
    public class UpdateCommentDTO
    {
        [Required] public int CommentId { get; set; }

        [Required] public string ApplicationUserId { get; set; }

        [Required] public string CommentContent { get; set; }
    }
}
