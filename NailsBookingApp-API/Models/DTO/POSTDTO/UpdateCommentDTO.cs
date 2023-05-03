using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO.POSTDTO
{
    public class UpdateCommentDTO
    {
        [Required] public int CommentId { get; set; }

        [Required] public string ApplicationUserId { get; set; }

        [Required] public string CommentContent { get; set; }
    }
}
