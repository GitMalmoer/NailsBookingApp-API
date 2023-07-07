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
using Microsoft.IdentityModel.Tokens;
using NailsBookingApp_API.Services;

namespace Application.MediatR.Auth.Commands
{
    public record RegisterCommand(RegisterRequestDTO registerRequestDto) : IRequest<ApiResponse>;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private ApiResponse _apiResponse;
        public RegisterCommandHandler(IAppDbContext dbContext, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailService = emailService;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

            ApplicationUser userFromDb = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == request.registerRequestDto.Email.ToLower());

            if (userFromDb != null)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("User name allready exists");
                return _apiResponse;
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                UserName = request.registerRequestDto.Email,
                Email = request.registerRequestDto.Email,
                Name = request.registerRequestDto.Name,
                LastName = request.registerRequestDto.LastName,
                NormalizedEmail = request.registerRequestDto.Email.ToUpper(),
                AccountCreateDate = DateTime.Now,
            };

            try
            {
                var result = await _userManager.CreateAsync(newUser, request.registerRequestDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);

                    //GENERATE EMAIL CONFIRMATION TOKEN
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    //var confirmationLink = Url.Action("ConfirmEmail", "Auth",
                    //  new { UserId = newUser.Id, Token = emailConfirmationToken }, Request.Scheme);

                    var token = Base64UrlEncoder.Encode(emailConfirmationToken);
                    var userEncoded = Base64UrlEncoder.Encode(newUser.Id);

                    var confirmationLink = $"{SD.actualWebsite}/confirmemail/?token={token}&user={userEncoded}";

                    _apiResponse.Result = confirmationLink; // TEST REMOVE

                    await _emailService.SendEmailVeryficationLink(confirmationLink, newUser.Email);

                    //ROLE IS CREATED IN APP DB INITIALIZER SEED METHOD
                    _apiResponse.IsSuccess = true;
                    //_apiResponse.Result = newUser; // JUST TEST REMOVE AFTER TESTING

                    _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    return _apiResponse;
                }
            }
            catch (Exception e)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(e.ToString());
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
            }

            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add(SD.error);
            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            return _apiResponse;
        }
    }
}
