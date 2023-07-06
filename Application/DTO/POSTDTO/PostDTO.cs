using System.ComponentModel.DataAnnotations;

namespace Application.DTO.POSTDTO
{
    public class PostDTO
    {
        [Required] public string ApplicationUserId { get; set; }


        [Required] public string Content { get; set; }

    }
}
