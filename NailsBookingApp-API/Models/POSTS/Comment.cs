using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.POSTS
{
    public class Comment
    {
        [Key] public int Id { get; set; }
        public virtual Post Post { get; set; }

        [Required]
        public int PostId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public ICollection<Like>? Likes { get; set; }


    }
}
