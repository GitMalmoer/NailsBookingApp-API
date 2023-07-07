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

namespace Application.MediatR.Post.Querries
{
    public record GetPostByIdQuerry(int id) : IRequest<ApiResponse>;

    public class GetPostByIdQuerryHandler : IRequestHandler<GetPostByIdQuerry, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public GetPostByIdQuerryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(GetPostByIdQuerry request, CancellationToken cancellationToken)
        {
            var post = await _dbContext
                .Posts
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Likes)
                .FirstOrDefaultAsync(p => p.Id == request.id);

            if (post == null || request.id <= 0)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("There is no post");
                return _apiResponse;
            }

            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = post;
            return _apiResponse;
        }
    }

}
