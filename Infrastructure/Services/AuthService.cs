using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApiResponse _apiResponse;
        private string _secretKey;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _apiResponse = new ApiResponse();
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<string> GenerateJwt(ApplicationUser user)
        {
            try
            {
                // GENERATE JWT
                var roles = await _userManager.GetRolesAsync(user);
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.UTF8.GetBytes(_secretKey);

                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("Id", user.Id),
                        new Claim("Name", user.Name),
                        new Claim("LastName", user.LastName ?? " "),
                        new Claim("Email", user.Email!),
                        new Claim("ConfirmedEmail", user.EmailConfirmed.ToString()),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    }),
                    Expires = DateTime.Now.AddDays(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

                string token = tokenHandler.WriteToken(securityToken);

                return token;
            }
            catch (Exception e)
            {
                return null;
            }

        }

    }
}
