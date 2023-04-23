using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NailsBookingApp_API.Models.POSTS
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey("Post")]
        public virtual Post? Post { get; set; }
        public int? PostId { get; set; }

        public virtual Comment? Comment { get; set; }
        public int? CommentId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        //[ForeignKey("ApplicationUser")]
        public Guid UserId { get; set; }

    }
}
