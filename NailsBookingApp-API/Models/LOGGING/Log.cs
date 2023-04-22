using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using NailsBookingApp_API.Models;
using System.Diagnostics;
using System.Security.Policy;

namespace NailsBookingApp_API.Models.LOGGING
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
        [MaxLength(10)]
        public string? Level { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public string? Exception { get; set; }
        [MaxLength(255)]
        public string? Logger { get; set; }
        [MaxLength(255)]
        public string? Url { get; set; }

    }
}


//CREATE TABLE Logs(
//    Id int NOT NULL PRIMARY KEY IDENTITY(1,1),
//    CreatedOn datetime NOT NULL,
//    Level nvarchar(10),
//    Message nvarchar(max),
//    StackTrace nvarchar(max),
//    Exception nvarchar(max),
//    Logger nvarchar(255),
//    Url nvarchar(255)
//);
