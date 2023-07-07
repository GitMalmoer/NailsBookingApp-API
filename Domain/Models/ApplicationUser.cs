using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }

        public string? PassResetToken { get; set; }
        public DateTime? PassResetExpirationDate { get; set; }
        public DateTime? AccountCreateDate { get; set; }
        
        public virtual AvatarPicture? AvatarPicture { get; set; }
        public int? AvatarPictureId { get; set; }

        public string? ExternalSubjectId { get; set; }

        public ApplicationUser()
        {
            AccountCreateDate = DateTime.Now;
            // the id of unknown user picture
            AvatarPictureId = 8;
        }


    }
}
