using System.ComponentModel.DataAnnotations;

namespace Domain.Models.DTO.POSTDTO
{
    public class PostDTO
    {
        [Required] public string ApplicationUserId { get; set; }


        [Required] public string Content { get; set; }

    }
}
