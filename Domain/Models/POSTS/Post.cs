using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.POSTS
{
    public class Post
    {

        [Key] 
        public int Id { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required] public string ApplicationUserId { get; set; }

        [Required] public DateTime CreateDateTime { get; set; }
        
        [Required] public string Content { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
    }
}
