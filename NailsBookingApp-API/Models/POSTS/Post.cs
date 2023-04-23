using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.POSTS
{
    public class Post
    {
        [Key] 
        public int Id { get; set; }
        [Required]
        public DateTime CreateDateTime { get; set; }
        
        [Required] public string Content { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
    }
}
