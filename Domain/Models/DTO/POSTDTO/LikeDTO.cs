using System.ComponentModel.DataAnnotations;

namespace Domain.Models.DTO.POSTDTO
{
    public class LikeDTO
    {
        [Required]
        public string ApplicationUserId { get; set; }
        public int? PostId { get; set; }
        public int? CommentId { get; set; }

    }
}
