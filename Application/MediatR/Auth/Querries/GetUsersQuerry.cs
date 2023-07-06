using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.ViewModels;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.Auth.Querries
{
    public record GetUsersQuerry : IRequest<ApiResponse>;

    public class GetUsersQuerryHandler : IRequestHandler<GetUsersQuerry, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;
        public GetUsersQuerryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(GetUsersQuerry request, CancellationToken cancellationToken)
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
                return _apiResponse;
            }

            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add("No users to show");
            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            return _apiResponse;
        }
    }

}
