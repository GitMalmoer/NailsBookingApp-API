using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailsBookingApp_API.Models;
using NailsBookingApp_API.Models.DTO;
using System.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using NailsBookingApp_API.Utility;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace NailsBookingApp_API.Controllers
{
    [Route("Api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ApiResponse _apiResponse;
        private string _secretKey;

        public AuthController(AppDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _apiResponse = new ApiResponse();
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");

        }

        [HttpPost("Register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequestDTO registerRequestDto)
        {
            ApplicationUser userFromDb = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == registerRequestDto.UserName.ToLower());

            if (userFromDb != null)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("User name allready exists");
                return BadRequest(_apiResponse);
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email,
                Name = registerRequestDto.Name,
                LastName = registerRequestDto.LastName,
                NormalizedEmail = registerRequestDto.Email.ToUpper(),
            };

            try
            {
                var result = await _userManager.CreateAsync(newUser, registerRequestDto.Password);

                if (result.Succeeded)
                {
                    // ROLE IS CREATED IN APPDBINITIALIZER SEED METHOD 
                    await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
                    _apiResponse.IsSuccess = true;
                    _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    return Ok(_apiResponse);
                }
            }
            catch (Exception e)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(e.ToString());
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add(SD.error);
            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            return BadRequest(_apiResponse);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDTO loginRequestDto)
        {
            ApplicationUser user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == loginRequestDto.UserName);
            if (user == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
                _apiResponse.ErrorMessages.Add("username or password are invalid");
                return NotFound(_apiResponse);
            }

            string accessToken = HttpContext.GetTokenAsync("access_token").Result;

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (isPasswordValid == false)
            {
                _apiResponse.Result = new LoginResponseDTO();
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
                _apiResponse.ErrorMessages.Add("username or password are invalid");
                return NotFound(_apiResponse);
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
                        new Claim("id", user.Id),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("LastName", user.LastName),
                        new Claim(ClaimTypes.Email, loginRequestDto.UserName),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    }),
                    Expires = DateTime.Now.AddDays(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
                SecurityToken securitytest = tokenHandler.CreateJwtSecurityToken(tokenDescriptor); // REMOVE

                LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
                {
                    Email = loginRequestDto.UserName,
                    Token = tokenHandler.WriteToken(securityToken),
                };

                if (loginResponseDTO.Email == null || string.IsNullOrEmpty(loginResponseDTO.Token))
                {
                    _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.ErrorMessages.Add("username or password is incorrect \n unidentified 444 error");
                    _apiResponse.IsSuccess = false;
                    return BadRequest(_apiResponse);
                }

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                _apiResponse.Result = loginResponseDTO;
                return Ok(_apiResponse);
            }
            catch (Exception e)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(e.ToString());
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }
        }

    }

}

