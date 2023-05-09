using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models
{
    public class AvatarPicture
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
