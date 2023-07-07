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
    public record GetCommentsByIdQuerry(int id) : IRequest<ApiResponse>;

    public class GetCommentsByIdQuerryHandler : IRequestHandler<GetCommentsByIdQuerry, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public GetCommentsByIdQuerryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(GetCommentsByIdQuerry request, CancellationToken cancellationToken)
        {
            var comments = _dbContext
                .Comments
                .Include(c => c.Likes)
                .Where(c => c.PostId == request.id)
                .Select(x => new CommentViewModel()
                {
                    Id = x.Id,
                    ApplicationUserName = x.ApplicationUser.Name,
                    ApplicationUserLastName = x.ApplicationUser.LastName,
                    ApplicationUserId = x.ApplicationUserId,
                    ApplicationUserAvatarUri = x.ApplicationUser.AvatarPicture.Path,
                    CreateDateTime = x.CreateDateTime,
                    PostId = x.PostId,
                    Likes = x.Likes,
                    CommentContent = x.CommentContent
                });

            if (comments != null)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                _apiResponse.Result = comments;
                return _apiResponse;
            }

            _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
            _apiResponse.ErrorMessages.Add("Comments Not Found");
            _apiResponse.IsSuccess = false;
            return _apiResponse;
        }
    }

}
