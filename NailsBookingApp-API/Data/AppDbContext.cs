
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NailsBookingApp_API.Models;
using NailsBookingApp_API.Models.LOGGING;

namespace NailsBookingApp_API
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers {get; set;}
        public DbSet<EmailQuestion> EmailQuestions { get; set;}

    }
}
