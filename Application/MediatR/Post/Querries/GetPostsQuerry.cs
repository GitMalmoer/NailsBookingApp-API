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

namespace Application.MediatR.Post.Querries
{
    public record GetPostsQuerry() : IRequest<ApiResponse>;

    public class GetPostsQuerryHandler : IRequestHandler<GetPostsQuerry, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public GetPostsQuerryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(GetPostsQuerry request, CancellationToken cancellationToken)
        {
            // MAYBE WE WILL NEED TO REMOVE INCLUDES JUST FOR OPTIMALIZATION 
            var posts = await _dbContext
                .Posts
                .Include(p => p.Likes)
                .Include(c => c.Comments)
                .Include(x => x.ApplicationUser).Select(x => new PostViewModel()
                {
                    Id = x.Id,
                    Content = x.Content,
                    Likes = x.Likes,
                    Comments = x.Comments,
                    CreateDateTime = x.CreateDateTime,
                    ApplicationUserId = x.ApplicationUserId,
                    ApplicationUserName = x.ApplicationUser.Name,
                    ApplicationUserLastName = x.ApplicationUser.LastName,
                    ApplicationUserAvatarUri = x.ApplicationUser.AvatarPicture.Path,
                })
                .ToListAsync();

            if (posts == null || !posts.Any())
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("There are no posts");
                _apiResponse.Result = posts;
                return _apiResponse;
            }

            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = posts;
            return _apiResponse;

        }
    }
}
