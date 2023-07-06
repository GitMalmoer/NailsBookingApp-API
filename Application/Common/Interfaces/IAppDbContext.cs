using Domain.Models.BOOKING;
using Domain.Models.LOGGING;
using Domain.Models.POSTS;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        DbSet<EmailQuestion> EmailQuestions { get; set; }
        DbSet<Log> Logs { get; set; }

        DbSet<Like> Likes { get; set; }
        DbSet<Post> Posts { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<AvatarPicture> AvatarPictures { get; set; }
        DbSet<Appointment> Appointments { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
