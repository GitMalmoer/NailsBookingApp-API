using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.DTO.POSTDTO;
using Domain.Models;
using Domain.Models.POSTS;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.Post.Commands
{
    public record HandleLikeCommand(LikeDTO likeDto) : IRequest<ApiResponse>;

    public class HandleLikeCommandHandler : IRequestHandler<HandleLikeCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public HandleLikeCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(HandleLikeCommand request, CancellationToken cancellationToken)
        {
            var userFromDb = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == request.likeDto.ApplicationUserId);

            if (userFromDb == null)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("userid is wrong");
                return _apiResponse;
            }

            // LIKING JUST A SINGLE COMMENT
            if ((request.likeDto.PostId == null || request.likeDto.PostId == 0) && request.likeDto.CommentId > 0)
            {
                var like = new Like()
                {
                    ApplicationUserId = request.likeDto.ApplicationUserId,
                    CommentId = request.likeDto.CommentId,
                };

                var commentFromDb = await _dbContext
                    .Comments
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(c => c.Id == request.likeDto.CommentId);

                var hasLiked = commentFromDb.Likes.FirstOrDefault(l => l.ApplicationUserId == request.likeDto.ApplicationUserId);

                if (hasLiked == null)
                {
                    _dbContext.Likes.Add(like);
                }
                else
                {
                    _dbContext.Likes.Remove(hasLiked);
                }
                await _dbContext.SaveChangesAsync(cancellationToken);

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return _apiResponse;

            }

            // LIKING JUST A SINGLE POST
            if (request.likeDto.PostId > 0 && (request.likeDto.CommentId == null || request.likeDto.CommentId == 0))
            {
                var like = new Like()
                {
                    ApplicationUserId = request.likeDto.ApplicationUserId,
                    PostId = request.likeDto.PostId
                };

                var postFromDb = await _dbContext
                    .Posts
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(c => c.Id == request.likeDto.PostId);

                var hasLiked = postFromDb.Likes.FirstOrDefault(l => l.ApplicationUserId == request.likeDto.ApplicationUserId);

                if (hasLiked == null)
                {
                    _dbContext.Likes.Add(like);
                }
                else
                {
                    _dbContext.Likes.Remove(hasLiked);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return _apiResponse;
            }


            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add("Like handle failed execution");
            return _apiResponse;
        }
    }


}
