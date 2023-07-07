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
    public record UpdateCommentCommand(UpdateCommentDTO updateCommentDto) : IRequest<ApiResponse>;

    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public UpdateCommentCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _dbContext.Comments.FindAsync(request.updateCommentDto.CommentId);

            if (comment == null || comment.ApplicationUserId != request.updateCommentDto.ApplicationUserId)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Comment not found or you are trying to update not your own comment.");
                return _apiResponse;
            }

            if (string.IsNullOrEmpty(request.updateCommentDto.CommentContent))
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Content cannot be empty");
                return _apiResponse;
            }

            comment.CommentContent = request.updateCommentDto.CommentContent;
            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _apiResponse.IsSuccess = true;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            return _apiResponse;
        }
    }

}
