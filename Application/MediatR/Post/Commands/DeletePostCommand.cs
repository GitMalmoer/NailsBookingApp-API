using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.DTO.POSTDTO;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.Post.Commands
{
    public record DeletePostCommand(DeletePostDTO deletePostDto) : IRequest<ApiResponse>;

    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;
        public DeletePostCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            // get the value from bearer 
            //var userId = User.FindFirst("Id")?.Value;

            var post = await _dbContext
                .Posts
                .Include(x => x.Comments).ThenInclude(x => x.Likes)
                .FirstOrDefaultAsync(p => p.Id == request.deletePostDto.PostId);


            if (post != null && post.ApplicationUserId == request.deletePostDto.ApplicationUserId)
            {
                var comments = await _dbContext.Comments.FirstOrDefaultAsync(x => x.PostId == request.deletePostDto.PostId);

                if (comments != null && comments.Likes != null && comments.Likes.Count > 0)
                {
                    _dbContext.Likes.RemoveRange(comments.Likes);
                }

                _dbContext.Posts.Remove(post);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return _apiResponse;
            }
            _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
            _apiResponse.ErrorMessages.Add("Post not found");
            _apiResponse.IsSuccess = false;
            return _apiResponse;
        }
    }

}
