namespace Domain.Models.LOGGING
{
    public class Log
    {
        public int Id { get; set; }

        public string MachineName { get; set; } = null!;

        public DateTime Logged { get; set; }

        public string Level { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string? Logger { get; set; }

        public string? Callsite { get; set; }

        public string? Exception { get; set; }

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
