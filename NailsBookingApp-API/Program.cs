
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NailsBookingApp_API.Models;
using System.Text;
using NailsBookingApp_API.Data;
using NailsBookingApp_API.Middleware;
using NailsBookingApp_API.Services;
using NLog;
using Stripe.BillingPortal;
using NLog.Web;

namespace NailsBookingApp_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");

            try
            {

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.

                builder.Services.AddControllers();


                builder.Services.AddTransient<ErrorHandlingMiddleware>();

                // SETTING UP CONNECTION WITH DB
                builder.Services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
                });


                // ADDING SERVICE OF IDENTITY
                builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();
                // ADD DEFAUTL TOKEN PRVIDERS IS USED FOR SENDING EMAIL WITH CONFIRMATION 
                //_userManager.GenerateEmailConfirmationTokenAsync

                // IDENTITY OPTIONS
                builder.Services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequiredLength = 1;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                });
                //AUTH
                var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
                // Add services to the container.
                builder.Services.AddAuthentication(u =>
                {
                    u.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    u.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(u =>
                {
                    u.RequireHttpsMetadata = false;
                    u.SaveToken = true;
                    u.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

                //ADD EMAIL CONFIG
                var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();

                // EMAIL CONFIG IS AS SINNGLETON
                builder.Services.AddSingleton(emailConfig);

                builder.Services.AddScoped<IEmailService, EmailService>();


                builder.Services.AddAuthorization();




                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();

                //SWGGER START
                builder.Services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                            "Example: \"Bearer 12345abcdef\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Scheme = JwtBearerDefaults.AuthenticationScheme
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                    });
                });
                // SWAGGER END

                // LOGGGING 
                //builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.UseMiddleware<ErrorHandlingMiddleware>();


                app.MapControllers();

                // APP DB SEEDER
                AppDbInitializer.SeedRolesAndUsers(app).Wait();

                app.Run();

            }
            catch (Exception exception)
            {
                // NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw (exception);
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }
    }
}