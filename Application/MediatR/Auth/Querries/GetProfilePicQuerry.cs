using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.Auth.Querries
{
    public record GetProfilePicQuerry(string userId) : IRequest<ApiResponse>;

    public class GetProfilePicQuerryHandler : IRequestHandler<GetProfilePicQuerry,ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public GetProfilePicQuerryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }

        public async Task<ApiResponse> Handle(GetProfilePicQuerry request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.ApplicationUsers
                .Include(x => x.AvatarPicture)
                .FirstOrDefaultAsync(u => u.Id == request.userId);

            if (user == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("User not found or Avatar not found");
                return _apiResponse;
            }
            else
            {
                _apiResponse.IsSuccess = true;
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.Result = user.AvatarPicture.Path;
                return _apiResponse;
            }
        }
    }

}
