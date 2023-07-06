using System.ComponentModel.DataAnnotations;

namespace Application.DTO.POSTDTO
{
    public class DeleteCommentDTO
    {
        [Required] public string ApplicationUserId { get; set; }
        [Required] public int CommentId { get; set; }
    }
}
