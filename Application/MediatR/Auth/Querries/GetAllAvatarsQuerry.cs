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
    public record GetAllAvatarsQuerry : IRequest<ApiResponse>;

    public class GetAllAvatarsQuerryHandler : IRequestHandler<GetAllAvatarsQuerry, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;
        public GetAllAvatarsQuerryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(GetAllAvatarsQuerry request, CancellationToken cancellationToken)
        {
            var avatars = _dbContext.AvatarPictures;

            if (avatars == null)
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
                _apiResponse.Result = avatars;
                return _apiResponse;
            }
        }
    }


}
