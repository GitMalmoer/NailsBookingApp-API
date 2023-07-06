using System.ComponentModel.DataAnnotations;

namespace Domain.Models.BOOKING
{
    public class CreateAppointmentDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        public string Time { get; set; }
        [Required]
        public int ServiceValue { get; set; }
        public double? Price { get; set; }
    }
}
