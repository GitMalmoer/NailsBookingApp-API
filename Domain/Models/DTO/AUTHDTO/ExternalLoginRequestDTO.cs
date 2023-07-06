using System.ComponentModel.DataAnnotations;

namespace Domain.Models.DTO.AUTHDTO
{
    public class ExternalLoginRequestDTO
    {
        [EmailAddress]
        public string Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public bool EmailConfirmed { get; set; }

        public string ExternalSubjectId { get; set; }

    }
}
