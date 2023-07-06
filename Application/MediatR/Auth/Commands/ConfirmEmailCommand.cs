using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using Application.DTO.AUTHDTO;

namespace Application.MediatR.Auth.Commands
{
    public record ConfirmEmailCommand(ConfirmEmailDTO confirmEmailDto) : IRequest<ApiResponse>;

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ApiResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApiResponse _apiResponse;
        public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.confirmEmailDto.userId) || string.IsNullOrEmpty(request.confirmEmailDto.token))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("user id or token is invalid");
                return _apiResponse;
            }

            string decodedUserId = Base64UrlEncoder.Decode(request.confirmEmailDto.userId);
            var userFromDb = await _userManager.FindByIdAsync(decodedUserId);

            if (userFromDb == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("user id or token invalid");
                return _apiResponse;
            }

            if (userFromDb.EmailConfirmed)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("Your email has been allready confirmed");
                return _apiResponse;
            }

            string decodedToken = Base64UrlEncoder.Decode(request.confirmEmailDto.token);
            var result = await _userManager.ConfirmEmailAsync(userFromDb, decodedToken);

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
                _apiResponse.ErrorMessages.Add("Error email could not be confirmed");
                return _apiResponse;
            }
        }
    }
}
