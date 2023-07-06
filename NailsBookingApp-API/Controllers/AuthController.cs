using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using NailsBookingApp_API.Services;
using Stripe;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using Application.DTO.AUTHDTO;
using Application.MediatR.Auth.Commands;
using Application.MediatR.Auth.Querries;
using Application.ViewModels;
using Domain.Models;
using Domain.Utility;
using Microsoft.AspNetCore.Authorization;
using NailsBookingApp_API.Services.AUTH;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities.Encoders;
using Infrastructure.Persistence;
using NailsBookingApp_API.Controllers.Base;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;
        private ApiResponse _apiResponse;
        private string _secretKey;

        public AuthController(AppDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService, IAuthService authService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _authService = authService;
            _apiResponse = new ApiResponse();
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequestDTO registerRequestDto)
        {
            ApiResponse result = await Mediator.Send(new RegisterCommand(registerRequestDto));

            return await HandleResult(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDTO loginRequestDto)
        {
            var result = await Mediator.Send(new LoginCommand(loginRequestDto));
            return await HandleResult(result);
        }

        [HttpPost("loginWithGoogle")]
        public async Task<ActionResult<ApiResponse>> LoginWithGoogle([FromBody] ExternalLoginRequestDTO externalLoginRequestDTO)
        {
            var result = await Mediator.Send(new LoginWithGoogleCommand(externalLoginRequestDTO));
            return await HandleResult(result);
        }

        [HttpPost("changePassword")]
        public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequestDTO changePasswordDTO)
        {
            var result = await Mediator.Send(new ChangePasswordCommand(changePasswordDTO));
            return await HandleResult(result);
        }
        /// <summary>
        /// The token and user has been decoded by register action and is decoded on the go in the confirmEmail action
        /// </summary>
        /// <param name="confirmEmailDto"></param>
        /// <returns></returns>
        [HttpPost("confirmEmail")]
        public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailDTO confirmEmailDto)
        {
            var result = await Mediator.Send(new ConfirmEmailCommand(confirmEmailDto));
            return await HandleResult(result);
        }

        [HttpPost("forgotPassword")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
        {
            var result = await Mediator.Send(new ForgotPasswordCommand(forgotPasswordDto));
            return await HandleResult(result);
        }

        [HttpPost("resetPassword")]
        public async Task<ActionResult<ApiResponse>> ResetPassword(PasswordResetDTO passwordResetDto)
        {
            var result = await Mediator.Send(new ResetPasswordCommand(passwordResetDto));
            return await HandleResult(result);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet("getUsers")]
        public async Task<ActionResult> GetUsers()
        {
            var result = await Mediator.Send(new GetUsersQuerry());
            return await HandleResult(result);
        }

        [HttpPost("profile/changeProfilePic")]
        public async Task<ActionResult<ApiResponse>> ChangeProfilePic(ChangeProfilePictureDTO changeProfilePicDto)
        {
            var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == changeProfilePicDto.UserId);
            var profilePic =
                await _dbContext.AvatarPictures.FirstOrDefaultAsync(x => x.Id == changeProfilePicDto.AvatarId);

            if (user == null || profilePic == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("User not found or Avatar not found");
                return BadRequest(_apiResponse);
            }

            user.AvatarPictureId = changeProfilePicDto.AvatarId;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            _apiResponse.IsSuccess = true;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpGet("profile/getProfilePic/{userId}")]
        public async Task<ActionResult<ApiResponse>> GetProfilePic(string userId)
        {
            var user = await _dbContext.ApplicationUsers
                .Include(x => x.AvatarPicture)
                .FirstOrDefaultAsync(u => u.Id == userId);


            if (user == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("User not found or Avatar not found");
                return BadRequest(_apiResponse);
            }
            else
            {
                _apiResponse.IsSuccess = true;
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.Result = user.AvatarPicture.Path;
                return Ok(_apiResponse);
            }
        }

        [HttpGet("profile/getAllAvatars")]
        public async Task<ActionResult<ApiResponse>> GetAllAvatars()
        {
            var avatars = _dbContext.AvatarPictures;

            if (avatars == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("User not found or Avatar not found");
                return BadRequest(_apiResponse);
            }
            else
            {
                _apiResponse.IsSuccess = true;
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.Result = avatars;
                return Ok(_apiResponse);
            }
        }
    }

}

