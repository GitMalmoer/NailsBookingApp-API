﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.DTO.AUTHDTO;
using Domain.Models;
using Domain.Utility;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Application.MediatR.Auth.Commands
{
    public record ForgotPasswordCommand(ForgotPasswordDTO forgotPasswordDto) : IRequest<ApiResponse>;

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand,ApiResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private ApiResponse _apiResponse;
        public ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.forgotPasswordDto.Email))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("Email not valid");
                return _apiResponse;
            }

            var userFromDb = await _userManager.FindByEmailAsync(request.forgotPasswordDto.Email);

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

                var passwordResetLink = $"{SD.actualWebsite}/resetpassword/token/?token=" + token;

                // SENDING PASSWORD RESET LINK USING SMTP
                await _emailService.SendPasswordResetLink(passwordResetLink, userFromDb.Email);

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                //_apiResponse.Result = passwordResetLink; just for test purposes
                return _apiResponse;
            }

            _apiResponse.IsSuccess = false;
            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            _apiResponse.ErrorMessages.Add("ERROR - Email not valid");
            return _apiResponse;
        }
    }
}
