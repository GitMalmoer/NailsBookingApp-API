using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.BOOKING
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
        public DateTime Date { get; set; }
        [Required]
        public string Time { get; set; }
        [Required]
        public int ServiceValue { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
