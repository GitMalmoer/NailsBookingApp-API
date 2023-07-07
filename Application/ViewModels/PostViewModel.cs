using Domain.Models.POSTS;

namespace Application.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public string ApplicationUserName{ get; set; }
        public string ApplicationUserLastName{ get; set; }
        public string ApplicationUserAvatarUri { get; set; }

        public DateTime CreateDateTime { get; set; }

        public string Content { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
    }
}
