using System.Net;
using Application.Common.Interfaces;
using Application.DTO.POSTDTO;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.Post.Commands
{
    public record CreatePostCommand(PostDTO postDto) : IRequest<ApiResponse>;

    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public CreatePostCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var userFromDb = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == request.postDto.ApplicationUserId);

            if (userFromDb == null || string.IsNullOrEmpty(request.postDto.Content))
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("userid is wrong or content is empty");
                return _apiResponse;
            }

            var post = _dbContext.Posts.Add(new Domain.Models.POSTS.Post()
            {
                ApplicationUserId = request.postDto.ApplicationUserId,
                Content = request.postDto.Content,
                CreateDateTime = DateTime.Now,
            });

            await _dbContext.SaveChangesAsync(cancellationToken);

            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return _apiResponse;
        }
    }
}
