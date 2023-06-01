﻿using System.ComponentModel.DataAnnotations;

namespace NailsBookingApp_API.Models.BOOKING
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int ServiceValue { get; set; }
        public double Price { get; set; }
    }
}
