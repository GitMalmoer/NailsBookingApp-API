using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NailsBookingApp_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }

        public string? PassResetToken { get; set; }
        public DateTime? PassResetExpirationDate { get; set; }
        public DateTime? AccountCreateDate { get; set; }

        [ForeignKey("AvatarPictureId")]
        public virtual AvatarPicture AvatarPicture { get; set; }
        public int AvatarPictureId { get; set; }

        public ApplicationUser()
        {
            AccountCreateDate = DateTime.Now;
            // the id of unknown user picture
            AvatarPictureId = 8;
        }

    }
}
