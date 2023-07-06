using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AvatarPicture
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
