using System.ComponentModel.DataAnnotations;

namespace Application.DTO.AUTHDTO
{
    /// <summary>
    /// Normal register request through application
    /// </summary>
    public class RegisterRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters are allowed in the name field.")]
        [MaxLength(15,ErrorMessage = "Maximum length of name is 15 letters")]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters are allowed in the last name field.")]
        [MaxLength(15, ErrorMessage = "Maximum length of last name is 15 letters")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [Compare("Password", ErrorMessage = "The password and confirm password do not match")]
        public string ConfirmPassword { get; set; }



    }
}
