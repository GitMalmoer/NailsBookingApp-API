using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.DTO.AUTHDTO;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.Auth.Commands
{
    public record ChangeProfilePicCommand(ChangeProfilePictureDTO changeProfilePicDto) : IRequest<ApiResponse>;

    public class ChangeProfilePicCommandHandler : IRequestHandler<ChangeProfilePicCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public ChangeProfilePicCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(ChangeProfilePicCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == request.changeProfilePicDto.UserId);
            var profilePic =
                await _dbContext.AvatarPictures.FirstOrDefaultAsync(x => x.Id == request.changeProfilePicDto.AvatarId);

            if (user == null || profilePic == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("User not found or Avatar not found");
                return _apiResponse;
            }

            user.AvatarPictureId = request.changeProfilePicDto.AvatarId;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _apiResponse.IsSuccess = true;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            return _apiResponse;
        }
    }

}
