using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.DTO.AUTHDTO;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.MediatR.Auth.Commands
{
    public record LoginCommand(LoginRequestDTO loginRequestDto) : IRequest<ApiResponse>;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApiResponse _apiResponse;
        private string _secretKey;
        public LoginCommandHandler(IAppDbContext dbContext,UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _apiResponse = new ApiResponse();
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }
        public async Task<ApiResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == request.loginRequestDto.UserName.ToLower());
            if (user == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
                _apiResponse.ErrorMessages.Add("username or password are invalid");
                return _apiResponse;
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.loginRequestDto.Password);

            if (isPasswordValid == false)
            {
                _apiResponse.Result = new LoginResponseDTO();
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
                _apiResponse.ErrorMessages.Add("username or password are invalid");
                return _apiResponse;
            }

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
                        new Claim("LastName", user.LastName),
                        new Claim("Email", request.loginRequestDto.UserName),
                        new Claim("ConfirmedEmail", user.EmailConfirmed.ToString()),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    }),
                    Expires = DateTime.Now.AddDays(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
                //SecurityToken securitytest = tokenHandler.CreateJwtSecurityToken(tokenDescriptor); // REMOVE

                LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
                {
                    Email = request.loginRequestDto.UserName,
                    Token = tokenHandler.WriteToken(securityToken),
                };

                if (loginResponseDTO.Email == null || string.IsNullOrEmpty(loginResponseDTO.Token))
                {
                    _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.ErrorMessages.Add("username or password is incorrect \n unidentified 444 error");
                    _apiResponse.IsSuccess = false;
                    return _apiResponse;
                }

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                _apiResponse.Result = loginResponseDTO;
                return _apiResponse;
            }
            catch (Exception e)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(e.ToString());
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
            }
        }
    }
}
