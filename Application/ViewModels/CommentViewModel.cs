using Domain.Models.POSTS;

namespace Application.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        public string ApplicationUserName { get; set; }
        public string ApplicationUserLastName { get; set; }
        public string ApplicationUserAvatarUri { get; set; }
        public string ApplicationUserId { get; set; }

        public DateTime CreateDateTime { get; set; }
        public string CommentContent { get; set; }
        public int PostId { get; set; }

        public ICollection<Like>? Likes { get; set; }
    }
}
