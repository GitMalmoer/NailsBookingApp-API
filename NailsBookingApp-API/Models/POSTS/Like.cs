using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NailsBookingApp_API.Models.POSTS
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("PostId")]
        public virtual Post? Post { get; set; }
        public int? PostId { get; set; }

        public virtual Comment? Comment { get; set; }
        public int? CommentId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }

    }
}
