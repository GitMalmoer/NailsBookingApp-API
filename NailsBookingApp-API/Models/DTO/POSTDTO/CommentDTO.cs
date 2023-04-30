using NailsBookingApp_API.Models.POSTS;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO.POSTDTO
{
    public class CommentDTO
    {
        [Required] public int PostId { get; set; }

        [Required] public string ApplicationUserId { get; set; }

        [Required] public string commentContent { get; set; }

    }
}
