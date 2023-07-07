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
    public record DeleteCommentCommand(DeleteCommentDTO deleteCommentDto) : IRequest<ApiResponse>;

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public DeleteCommentCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            //var userId = User.FindFirst("Id")?.Value;

            var comment = await _dbContext
                .Comments
                .Include(x => x.Likes)
            .FirstOrDefaultAsync(c => c.Id == request.deleteCommentDto.CommentId);


            if (comment != null && comment.ApplicationUserId == request.deleteCommentDto.ApplicationUserId)
            {
                //var commentLikes = await _dbContext.Comments.FirstOrDefaultAsync(x => x.PostId == deletePostDto.PostId);
                //_dbContext.Likes.RemoveRange(commentLikes.Likes);

                _dbContext.Comments.Remove(comment);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return _apiResponse;
            }
            _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
            _apiResponse.IsSuccess = false;
            return _apiResponse;
        }
    }
}
