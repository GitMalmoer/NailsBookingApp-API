using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NailsBookingApp_API.Models.POSTS
{
    public class Comment
    {
        [Key] public int Id { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }
        // virtual enables lazy loading - which means that the related Post entity will be loaded from the database only when it is actually accessed.
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }

        [Required]
        public string CommentContent { get; set; }

        [Required]
        public int PostId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public  ICollection<Like>? Likes { get; set; }


    }
}
