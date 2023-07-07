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
    public record AddCommentCommand(CommentDTO commentDTO) : IRequest<ApiResponse>;

    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public AddCommentCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }

        public async Task<ApiResponse> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var userFromDb = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == request.commentDTO.ApplicationUserId);
            if (userFromDb == null || string.IsNullOrEmpty(request.commentDTO.commentContent) || request.commentDTO.PostId == null ||
                request.commentDTO.PostId <= 0)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("userid is wrong or content is empty");
                return _apiResponse;
            }

            var comment = _dbContext.Comments.Add(new Comment()
            {
                ApplicationUserId = request.commentDTO.ApplicationUserId,
                CommentContent = request.commentDTO.commentContent,
                CreateDateTime = DateTime.Now,
                PostId = request.commentDTO.PostId,
            });

            await _dbContext.SaveChangesAsync(cancellationToken);

            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return _apiResponse;
        }
    }
}
