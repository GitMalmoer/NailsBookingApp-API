using System.Collections;
using Application.Common.Interfaces;
using Azure.Storage.Blobs;
using Domain.Models;
using Domain.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public static class AppDbInitializer
    {
        public static async Task SeedAvatarPictures(IApplicationBuilder applicationBuilder)
        {
            using (var scope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<AppDbContext>();
                var blobService = scope.ServiceProvider.GetService<IBlobService>();


                if (!context.AvatarPictures.Any())
                {
                    IEnumerable<AvatarPicture> avatars = await blobService.ListAvatars(SD.blobContainerName);

                    await context.AddRangeAsync(avatars);
                    await context.SaveChangesAsync();
                }

            }
        }

        public static async Task SeedRolesAndUsers(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                //context.Database.EnsureCreated();

                if (context.Database.CanConnect())
                {
                    if (context.Database.IsRelational())
                    {
                        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

                        if (pendingMigrations.Any())
                        {
                            context.Database.Migrate();
                        }
                    }
                }



                var _roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var _userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                }

                if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                }

                var userInDb =
                    await context.ApplicationUsers.FirstOrDefaultAsync(u =>
                        u.UserName.ToLower() == SD.Role_Admin.ToLower());

                if (userInDb == null)
                {
                    var newUser = new ApplicationUser()
                    {
                        Name = SD.Role_Admin,
                        LastName = SD.Role_Admin,
                        UserName = SD.Role_Admin,
                        Email = SD.Role_Admin,
                        EmailConfirmed = true,
                        AvatarPictureId = 8,
                    };
                    await _userManager.CreateAsync(newUser, "admin");
                    await _userManager.AddToRoleAsync(newUser, SD.Role_Admin);
                }

            }
        }


    }
}
