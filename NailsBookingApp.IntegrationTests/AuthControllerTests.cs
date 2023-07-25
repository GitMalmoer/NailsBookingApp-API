using System.Net;
using Application.DTO.AUTHDTO;
using Application.MediatR.Auth.Commands;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NailsBookingApp_API;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace NailsBookingApp.IntegrationTests
{
    public class AuthControllerTests 
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private HttpClient _client;
        private WebApplicationFactory<Program> _factory;
        public AuthControllerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _factory = new WebApplicationFactory<Program>();

            _factory = _factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                var dbContextOptions = services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<AppDbContext>));

                services.Remove(dbContextOptions);

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
            RegisterRequestDTO registerRequestDto = new RegisterRequestDTO()
            {
                Name = "Test",
                LastName = "Test",
                Email = "emailtest@gmail.com",
                Password = "Password12#",
                ConfirmPassword = "Password12#"
            };


            LoginRequestDTO loginRequestDto = new LoginRequestDTO()
            {
                UserName = "emailtest@gmail.com",
                Password = "Password12#",
            };

            var registerHttpContent = registerRequestDto.ChangeModelToHttpContent();
            var loginHttpContent = loginRequestDto.ChangeModelToHttpContent();

            //act
            var registerResult = await _client.PostAsync("api/auth/register", registerHttpContent);
            var loginResult = await _client.PostAsync("api/auth/login", loginHttpContent);
            var content = await loginResult.Content.ReadAsStringAsync();
            var contentTest = loginResult.Content.ReadAsStream();

            
            ApiResponse loginResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
            _testOutputHelper.WriteLine(loginResponse.Result.ToString());

            //assert
            loginResponse.Should().NotBeNull();
            registerResult.StatusCode.Should().Be(HttpStatusCode.OK);
            loginResult.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private void SeedDbWithUser(RegisterRequestDTO registerRequestDto)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<AppDbContext>();

            ApplicationUser user = new ApplicationUser()
            {
                Id = "idofuser",
                Email = registerRequestDto.Email,
                UserName = registerRequestDto.Email,
                Name = registerRequestDto.Name,
                LastName = registerRequestDto.LastName,

            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task RegisterUser_IfUserAllreadyExists_ReturnsBadRequest()
        {
            // arrange
            RegisterRequestDTO user = new RegisterRequestDTO()
            {
                Name = "Test",
                LastName = "Test",
                Email = "emailtest@gmail.com",
                Password = "Password12#",
                ConfirmPassword = "Password12#"
            };

            SeedDbWithUser(user);

            var notUniqueUserHttpContent = user.ChangeModelToHttpContent();

            //act
            var notUniqueRegisterResult = await _client.PostAsync("api/auth/register", notUniqueUserHttpContent);

            // assert
            notUniqueRegisterResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);


        }

    }
}