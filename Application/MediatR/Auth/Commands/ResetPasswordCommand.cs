using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.AUTHDTO;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Application.MediatR.Auth.Commands
{
    public record ResetPasswordCommand(PasswordResetDTO passwordResetDto) : IRequest<ApiResponse>;

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApiResponse _apiResponse;
        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.passwordResetDto.email) || string.IsNullOrEmpty(request.passwordResetDto.token) || request.passwordResetDto == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("You provided not valid data (null)");
                return _apiResponse;
            }

            var userFromDb = await _userManager.FindByEmailAsync(request.passwordResetDto.email);

            if (userFromDb != null)
            {
                // CHECK IF TOKEN IN DTO IS THE SAME AS ONE IN DATABASE AND IF DATE TIME NOW IS LOWER THAN THE DATETIME IN DB
                if (userFromDb.PassResetToken == request.passwordResetDto.token && DateTime.Now <= userFromDb.PassResetExpirationDate)
                {
                    // TOKEN WAS ENCODED TO SIMPLIFY THE URI! NOW ITS GETTING DECODED FOR THE USER MANAGER
                    var decodedToken = Base64UrlEncoder.Decode(request.passwordResetDto.token);
                    var result = await _userManager.ResetPasswordAsync(userFromDb, decodedToken, request.passwordResetDto.Password);
                    if (result.Succeeded)
                    {
                        _apiResponse.IsSuccess = true;
                        _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                        return _apiResponse;
                    }
                    else
                    {
                        _apiResponse.IsSuccess = false;
                        _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                        _apiResponse.ErrorMessages.Add("Error during password reset");
                        return _apiResponse;
                    }
                }
                else
                {
                    _apiResponse.ErrorMessages.Add("Your token is no longer valid");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

            }

            _apiResponse.IsSuccess = false;
            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            _apiResponse.ErrorMessages.Add("You provided not valid data");
            return _apiResponse;
        }
    }

}
