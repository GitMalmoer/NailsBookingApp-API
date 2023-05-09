using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailsBookingApp_API.Models;
using System.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using NailsBookingApp_API.Utility;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using NailsBookingApp_API.Models.DTO.AUTHDTO;
using NailsBookingApp_API.Models.ViewModels;
using NailsBookingApp_API.Services;
using Stripe;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities.Encoders;

namespace NailsBookingApp_API.Controllers
{
    [Route("Api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private ApiResponse _apiResponse;
        private string _secretKey;

        public AuthController(AppDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _apiResponse = new ApiResponse();
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");

        }

        [HttpPost("Register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequestDTO registerRequestDto)
        {

            if (!ModelState.IsValid)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Model state is not valid");
                _apiResponse.ErrorMessages.Add(ModelState.Values.ToString());
                return BadRequest(_apiResponse);
            }

            ApplicationUser userFromDb = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == registerRequestDto.Email.ToLower());

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
                AccountCreateDate = DateTime.Now,
            };

            try
            {
                var result = await _userManager.CreateAsync(newUser, registerRequestDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);

                    //GENERATE EMAIL CONFIRMATION TOKEN
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    //var confirmationLink = Url.Action("ConfirmEmail", "Auth",
                    //  new { UserId = newUser.Id, Token = emailConfirmationToken }, Request.Scheme);

                    var token = Base64UrlEncoder.Encode(emailConfirmationToken);
                    var userEncoded = Base64UrlEncoder.Encode(newUser.Id);

                    var confirmationLink = $"http://localhost:3000/confirmemail/?token={token}&user={userEncoded}";

                    _apiResponse.Result = confirmationLink; // TEST REMOVE

                    await _emailService.SendEmailVeryficationLink(confirmationLink, newUser.Email);
                    
                    //ROLE IS CREATED IN APP DB INITIALIZER SEED METHOD
                    _apiResponse.IsSuccess = true;
                    //_apiResponse.Result = newUser; // JUST TEST REMOVE AFTER TESTING

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
                        new Claim("Id", user.Id),
                        new Claim("Name", user.Name),
                        new Claim("LastName", user.LastName),
                        new Claim("Email", loginRequestDto.UserName),
                        new Claim("ConfirmedEmail", user.EmailConfirmed.ToString()),
                        new Claim("Role", roles.FirstOrDefault()),
                    }),
                    Expires = DateTime.Now.AddDays(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
                //SecurityToken securitytest = tokenHandler.CreateJwtSecurityToken(tokenDescriptor); // REMOVE

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

        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequestDTO changePasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Model state is not valid");
                _apiResponse.ErrorMessages.AddRange(ModelState
                    .Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList());
                return BadRequest(_apiResponse);
            }

            // IN FRONTEND GET THIS VALUE FROM DECODING JWT
            ApplicationUser userFromDb =
                await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == changePasswordDTO.email);

            if (userFromDb != null)
            {
                if (changePasswordDTO.NewPassword == changePasswordDTO.OldPassword)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("Old password and new Password are the same");
                    _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var result = await _userManager.ChangePasswordAsync(userFromDb, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
                if (result.Succeeded)
                {
                    _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    return Ok(_apiResponse);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _apiResponse.ErrorMessages.Add(error.Description);
                    }
                    _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    return BadRequest(_apiResponse);
                }

            }

            _apiResponse.IsSuccess = false;
            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            _apiResponse.ErrorMessages.Add("User does not exist");
            return NotFound(_apiResponse);

        }
        /// <summary>
        /// The token and user has been decoded by register action and is decoded on the go in the confirmEmail action
        /// </summary>
        /// <param name="confirmEmailDto"></param>
        /// <returns></returns>
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailDTO confirmEmailDto)
        {
            if (string.IsNullOrEmpty(confirmEmailDto.userId) || string.IsNullOrEmpty(confirmEmailDto.token))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("user id or token is invalid");
                return NotFound(_apiResponse);
            }

            string decodedUserId = Base64UrlEncoder.Decode(confirmEmailDto.userId);
            var userFromDb = await _userManager.FindByIdAsync(decodedUserId);

            if (userFromDb == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("user id or token invalid");
                return NotFound(_apiResponse);
            }

            if (userFromDb.EmailConfirmed)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("Your emails has been allready confirmed");
                return BadRequest(_apiResponse);
            }

            string decodedToken = Base64UrlEncoder.Decode(confirmEmailDto.token);
            var result = await _userManager.ConfirmEmailAsync(userFromDb, decodedToken);

            if (result.Succeeded)
            {
                _apiResponse.IsSuccess = true;
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            else
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("Error email could not be confirmed");
                return BadRequest(_apiResponse);
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
        {
            if (string.IsNullOrEmpty(forgotPasswordDto.Email))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("Email not valid");
                return NotFound(_apiResponse);
            }

            var userFromDb = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (userFromDb != null)
            {
                // VERY IMPORTANT PART - TOKEN NEEDS TO BE ENCODED BECAUSE IN REACT APP '+' '/' SIGNS ARE TRANSFORMED INTO %20 OR OTHER SYMBOLS.
                // LATER ON WHEN PASSWORD IS BEING RESETED - WE DECODE THE TOKEN.
                var tokenFromUserManager = await _userManager.GeneratePasswordResetTokenAsync(userFromDb);
                var token = Base64UrlEncoder.Encode(tokenFromUserManager);

                // this solutions beneath provides us with token but usermanager.resetpasswordasync wont process this token beccause it was not generated by usermanager.
                //byte[] tokenData = new byte[64];
                //using (var rng = RandomNumberGenerator.Create())
                //{
                //    rng.GetBytes(tokenData);
                //}
                //string token = WebEncoders.Base64UrlEncode(tokenData);


                // SETTING UP TOKEN IN DATABASE
                userFromDb.PassResetToken = token;
                // EXPRIATION TIME OF TOKEN - 10 MINUTES
                userFromDb.PassResetExpirationDate = DateTime.Now.AddMinutes(10);
                // SAVE CHANGES
                await _userManager.UpdateAsync(userFromDb);

                //var passwordResetLink = Url.Action("ResetPassword", "Auth",
                //    new { Email = forgotPasswordDto.Email, Token = token }, Request.Scheme);

                var passwordResetLink = "http://localhost:3000/resetpassword/token/?token=" + token;

                // SENDING PASSWORD RESET LINK USING SMTP
                await _emailService.SendPasswordResetLink(passwordResetLink, userFromDb.Email);

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                _apiResponse.Result = passwordResetLink;
                return Ok(_apiResponse);
            }

            _apiResponse.IsSuccess = false;
            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            _apiResponse.ErrorMessages.Add("ERROR - Email not valid");
            return NotFound(_apiResponse);

        }

        [HttpGet("ResetPassword")]
        public async Task<ActionResult<ApiResponse>> ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("You provided not valid data");
                return BadRequest(_apiResponse);
            }

            PasswordResetDTO passwordResetDto = new PasswordResetDTO()
            {
                token = token,
                email = email,
            };

            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = passwordResetDto;
            return Ok(_apiResponse);
        }


        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ApiResponse>> ResetPassword(PasswordResetDTO passwordResetDto)
        {
            if (string.IsNullOrEmpty(passwordResetDto.email) || string.IsNullOrEmpty(passwordResetDto.token) || passwordResetDto == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("You provided not valid data (null)");
                return BadRequest(_apiResponse);
            }

            var userFromDb = await _userManager.FindByEmailAsync(passwordResetDto.email);

            if (userFromDb != null)
            {
                // CHECK IF TOKEN IN DTO IS THE SAME AS ONE IN DATABASE AND IF DATE TIME NOW IS LOWER THAN THE DATETIME IN DB
                if (userFromDb.PassResetToken == passwordResetDto.token && DateTime.Now <= userFromDb.PassResetExpirationDate)
                {
                    // TOKEN WAS ENCODED TO SIMPLIFY THE URI! NOW ITS GETTING DECODED FOR THE USER MANAGER
                    var decodedToken = Base64UrlEncoder.Decode(passwordResetDto.token);
                    var result = await _userManager.ResetPasswordAsync(userFromDb, decodedToken, passwordResetDto.Password);
                    if (result.Succeeded)
                    {
                        _apiResponse.IsSuccess = true;
                        _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                        return Ok(_apiResponse);
                    }
                    else
                    {
                        return BadRequest("Something went wrong");
                    }
                }
                else
                {
                    _apiResponse.ErrorMessages.Add("Your token is no longer valid");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

            }

            _apiResponse.IsSuccess = false;
            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            _apiResponse.ErrorMessages.Add("You provided not valid data");
            return BadRequest(_apiResponse);
        }

        [HttpGet("GetUsers")]
        public async Task<ActionResult> GetUsers()
        {
            var users = _dbContext.ApplicationUsers.Select(u => new UserViewModel()
            {
                EmailConfirmed = u.EmailConfirmed,
                Name = u.Name,
                Id = u.Id,
                Email = u.Email,
                LastName = u.LastName,
            });

            if (users.Any())
            {
                _apiResponse.IsSuccess = true;
                _apiResponse.Result = users;
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }

            _apiResponse.IsSuccess = false;
            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            return BadRequest(_apiResponse);
        }

    }

}

