using NailsBookingApp_API.Models.POSTS;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.DTO.POSTDTO
{
    public class LikeDTO
    {
        [Required]
        public string ApplicationUserId { get; set; }
        public int? PostId { get; set; }
        public int? CommentId { get; set; }

    }
}
