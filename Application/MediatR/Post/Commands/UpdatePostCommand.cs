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
    public record UpdatePostCommand(UpdatePostDTO updatePostDto) : IRequest<ApiResponse>;

    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public UpdatePostCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApiResponse> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _dbContext.Posts.FindAsync(request.updatePostDto.PostId);

            if (post == null || post.ApplicationUserId != request.updatePostDto.ApplicationUserId)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Post not found or you are trying to update not your own post.");
                return _apiResponse;
            }

            if (string.IsNullOrEmpty(request.updatePostDto.Content))
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Content cannot be empty");
                return _apiResponse;
            }

            post.Content = request.updatePostDto.Content;
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _apiResponse.IsSuccess = true;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            return _apiResponse;
        }
    }

}
