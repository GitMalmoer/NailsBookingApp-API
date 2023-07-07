using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.DTO.AUTHDTO;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.Auth.Commands
{
    public record ChangePasswordCommand(ChangePasswordRequestDTO changePasswordDTO) : IRequest<ApiResponse>;

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApiResponse _apiResponse;
        public ChangePasswordCommandHandler(IAppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // IN FRONTEND GET THIS VALUE FROM DECODING JWT
            ApplicationUser userFromDb =
                await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == request.changePasswordDTO.email);

            if (userFromDb != null)
            {
                if (request.changePasswordDTO.NewPassword == request.changePasswordDTO.OldPassword)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("Old password and new Password are the same");
                    _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                var result = await _userManager.ChangePasswordAsync(userFromDb, request.changePasswordDTO.OldPassword, request.changePasswordDTO.NewPassword);
                if (result.Succeeded)
                {
                    _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    return _apiResponse;
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _apiResponse.ErrorMessages.Add(error.Description);
                    }
                    _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    return _apiResponse;
                }

            }

            _apiResponse.IsSuccess = false;
            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            _apiResponse.ErrorMessages.Add("User does not exist");
            return _apiResponse;
        }
    }

}
