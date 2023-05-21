
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NailsBookingApp_API.Models;
using NailsBookingApp_API.Models.LOGGING;
using System.Reflection.Emit;
using NailsBookingApp_API.Models.POSTS;

namespace NailsBookingApp_API
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<EmailQuestion> EmailQuestions { get; set; }
        public virtual DbSet<Log> Logs { get; set; }

        public DbSet<Like> Likes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AvatarPicture> AvatarPictures { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.Log");

                entity.ToTable("Log");

                entity.Property(e => e.Level).HasMaxLength(50);
                entity.Property(e => e.Logged).HasColumnType("datetime");
                entity.Property(e => e.Logger).HasMaxLength(250);
                entity.Property(e => e.MachineName).HasMaxLength(50);
            });

            builder.Entity<Comment>()
                .HasOne(c => c.ApplicationUser)
                .WithMany()
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Like>()
                .HasOne(l => l.ApplicationUser)
                .WithMany()
                .HasForeignKey(l => l.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Like>()
                .HasOne(l => l.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(l => l.CommentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Post>()
                .HasMany(p => p.Likes)
                .WithOne(l => l.Post)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<Comment>()
            //    .HasMany(x => x.Likes)
            //    .WithOne(l => l.Comment)
            //    .HasForeignKey(l => l.CommentId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<ApplicationUser>()
            //    .HasOne<AvatarPicture>()
            //    .WithMany()
            //    .HasForeignKey(x => x.AvatarPictureId)
            //    .OnDelete(DeleteBehavior.NoAction);

            // DEFAULT VALUE - ID 8 TARGETS UNKNOWN PROFILE PICTURE IN DATABASE
            builder.Entity<ApplicationUser>().Property(x => x.AvatarPictureId).HasDefaultValue(8);


        }
    }
}
