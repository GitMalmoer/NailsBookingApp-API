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

namespace Application.MediatR.Logging
{
    public record GetErrorLogsQuerry : IRequest<ApiResponse>;

    public class GetErrorLogsQuerryHandler : IRequestHandler<GetErrorLogsQuerry, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public GetErrorLogsQuerryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(GetErrorLogsQuerry request, CancellationToken cancellationToken)
        {
            //var userId = User.FindFirst("Id")?.Value;

            var logs = _dbContext.Logs.Where(l => l.Level == "error");
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = logs;
            return _apiResponse;
        }
    }


}
