using Microsoft.AspNetCore.Mvc;
using Application.DTO.AUTHDTO;
using Application.MediatR.Auth.Commands;
using Application.MediatR.Auth.Querries;
using Domain.Models;
using Domain.Utility;
using Microsoft.AspNetCore.Authorization;
using NailsBookingApp_API.Controllers.Base;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
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
            var result = await Mediator.Send(new ChangeProfilePicCommand(changeProfilePicDto));
            return await HandleResult(result);
        }

        [HttpGet("profile/getProfilePic/{userId}")]
        public async Task<ActionResult<ApiResponse>> GetProfilePic(string userId)
        {
            var result = await Mediator.Send(new GetProfilePicQuerry(userId));
            return await HandleResult(result);
        }

        [HttpGet("profile/getAllAvatars")]
        public async Task<ActionResult<ApiResponse>> GetAllAvatars()
        {
            var result = await Mediator.Send(new GetAllAvatarsQuerry());
            return await HandleResult(result);
        }
    }

}

