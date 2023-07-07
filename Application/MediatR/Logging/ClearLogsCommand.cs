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
    public record ClearLogsCommand : IRequest<ApiResponse>;

    public class ClearLogsCommandHandler : IRequestHandler<ClearLogsCommand, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public ClearLogsCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(ClearLogsCommand request, CancellationToken cancellationToken)
        {
            var logs = _dbContext.Logs;
            _dbContext.RemoveRange(logs);

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            _apiResponse.HttpStatusCode = HttpStatusCode.NoContent;

            if (result > 0)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
            }
            return _apiResponse;
        }
    }
}
