using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.EmailMessage.Querries
{
    public record GetMessagesQuerry() : IRequest<ApiResponse>;

    public class GetMessagesQuerryHandler : IRequestHandler<GetMessagesQuerry, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public GetMessagesQuerryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(GetMessagesQuerry request, CancellationToken cancellationToken)
        {
            var emailMessages = _dbContext.EmailQuestions;

            if (emailMessages.Any())
            {
                _apiResponse.IsSuccess = true;
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.Result = emailMessages;
                return _apiResponse;
            }


            _apiResponse.IsSuccess = false;
            _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
            _apiResponse.ErrorMessages.Add("No messages found");
            return _apiResponse;
        }
    }

}
