using System;
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
using Microsoft.EntityFrameworkCore;


namespace Application.MediatR.Auth.Commands
{
    public record LoginWithGoogleCommand(ExternalLoginRequestDTO externalLoginRequestDTO) : IRequest<ApiResponse>;

    public class LoginWithGoogleCommandHandler : IRequestHandler<LoginWithGoogleCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        private ApiResponse _apiResponse;

        public LoginWithGoogleCommandHandler(IAppDbContext dbContext, UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _authService = authService;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == request.externalLoginRequestDTO.Email.ToLower());
            if (user == null)
            {
                ApplicationUser newUser = new ApplicationUser()
                {
                    UserName = request.externalLoginRequestDTO.Email,
                    Email = request.externalLoginRequestDTO.Email,
                    Name = request.externalLoginRequestDTO.FirstName,
                    LastName = request.externalLoginRequestDTO.LastName,
                    EmailConfirmed = request.externalLoginRequestDTO.EmailConfirmed,
                    NormalizedEmail = request.externalLoginRequestDTO.Email.ToUpper(),
                    ExternalSubjectId = request.externalLoginRequestDTO.ExternalSubjectId,
                    AccountCreateDate = DateTime.Now,
                };

                var result = await _userManager.CreateAsync(newUser);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
                    user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == request.externalLoginRequestDTO.Email.ToLower());

                    string token = await _authService.GenerateJwt(user);
                    if (token != null)
                    {
                        _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                        _apiResponse.IsSuccess = true;
                        _apiResponse.Result = token;
                        return _apiResponse;
                    }
                }

            }
            else
            {
                if ((user.ExternalSubjectId != null && user.ExternalSubjectId.Length > 0) && user.ExternalSubjectId == request.externalLoginRequestDTO.ExternalSubjectId)
                {
                    var token = await _authService.GenerateJwt(user);
                    if (token != null)
                    {
                        _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                        _apiResponse.IsSuccess = true;
                        _apiResponse.Result = token;
                        return _apiResponse;
                    }
                }
                else if (user.ExternalSubjectId == null)
                {

                }
            }

            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add("Error! Try Again");
            return _apiResponse;
        }
    }
}
