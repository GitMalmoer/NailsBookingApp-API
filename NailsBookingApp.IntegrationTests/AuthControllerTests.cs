using System.Net;
using Application.Common.Interfaces;
using Application.DTO.AUTHDTO;
using Application.MediatR.Auth.Commands;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NailsBookingApp_API;
using Newtonsoft.Json;
using Stripe;
using Xunit.Abstractions;

namespace NailsBookingApp.IntegrationTests
{
    public class AuthControllerTests 
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private HttpClient _client;
        private WebApplicationFactory<Program> _factory;
        private Mock<IAuthService> _authServiceMock = new Mock<IAuthService>();
        public AuthControllerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _factory = new WebApplicationFactory<Program>();

            _factory = _factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                var dbContextOptions = services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<AppDbContext>));

                services.Remove(dbContextOptions);

                services.AddSingleton<IAuthService>(_authServiceMock.Object);

                services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("MemoryDb"));
            }));

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task RegisterUser_ForValidRegisterDto_ReturnsOk()
        {
            // arrange
            RegisterRequestDTO registerRequestDto = new RegisterRequestDTO()
            {
                Name = "Test",
                LastName = "Test",
                Email = "emailtest@gmail.com",
                Password = "Password12#",
                ConfirmPassword = "Password12#"
            };

            var httpContent = registerRequestDto.ChangeModelToHttpContent();

            //act
            var result = await _client.PostAsync("api/auth/register", httpContent);

            //assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task LoginUser_ForValidRegisterDto_ReturnsOkAndJwt()
        {
            // arrange
            ApplicationUser user = new ApplicationUser()
            {
                Name = "Test",
                LastName = "Test",
                Email = "emailtest@gmail.com",
            };

            ApplicationUser userFromDb = SeedDbWithUser(user,"Password12#");

            _authServiceMock.Setup(s => s.GenerateJwt(It.IsAny<ApplicationUser>())).ReturnsAsync("jwt");

            LoginRequestDTO loginRequestDto = new LoginRequestDTO()
            {
                UserName = "emailtest@gmail.com",
                Password = "Password12#",
            };

            var loginHttpContent = loginRequestDto.ChangeModelToHttpContent();

            //act
            var loginResult = await _client.PostAsync("api/auth/login", loginHttpContent);
            var content = await loginResult.Content.ReadAsStringAsync();


            ApiResponse loginResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
            _testOutputHelper.WriteLine(loginResponse.Result.ToString());

            //assert
            loginResponse.Should().NotBeNull();
            loginResult.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private ApplicationUser SeedDbWithUser(ApplicationUser applicationUser, string password)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<AppDbContext>();

            PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();

            ApplicationUser user = new ApplicationUser()
            {
                Id = "idofuser",
                Email = applicationUser.Email,
                UserName = applicationUser.Email,
                Name = applicationUser.Name,
                LastName = applicationUser.LastName,
                EmailConfirmed = true,
            };

            var hashedPassword = hasher.HashPassword(user, password);
            user.PasswordHash = hashedPassword;

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return user;
        }

        [Fact]
        public async Task RegisterUser_IfUserAllreadyExists_ReturnsBadRequest()
        {
            // arrange
            RegisterRequestDTO newUser = new RegisterRequestDTO()
            {
                Name = "Test",
                LastName = "Test",
                Email = "emailtest@gmail.com",
                Password = "Password12#",
                ConfirmPassword = "Password12#"
            };

            ApplicationUser existingUser = new ApplicationUser()
            {
                Name = "Test",
                LastName = "Test",
                Email = "emailtest@gmail.com",
            };

            SeedDbWithUser(existingUser, "Password12#");

            var notUniqueUserHttpContent = newUser.ChangeModelToHttpContent();
            //act
            var notUniqueRegisterResult = await _client.PostAsync("api/auth/register", notUniqueUserHttpContent);

            // assert
            notUniqueRegisterResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);


        }

    }
}