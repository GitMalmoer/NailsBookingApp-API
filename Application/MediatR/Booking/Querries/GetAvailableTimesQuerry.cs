using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Models;
using Domain.Models.BOOKING;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.Booking.Querries
{
    public record GetAvailableTimesQuerry(string stringDate) : IRequest<ApiResponse>;
    
    public class GetAvailableTimesQuerryHandler : IRequestHandler<GetAvailableTimesQuerry, ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;
        public GetAvailableTimesQuerryHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }

        public async Task<ApiResponse> Handle(GetAvailableTimesQuerry request, CancellationToken cancellationToken)
        {
            bool isDateOk = DateTime.TryParse(request.stringDate, out DateTime selectedDate);

            if (!isDateOk)
            {
                _apiResponse.ErrorMessages.Add("Wrong date");
                _apiResponse.IsSuccess = false;
                return _apiResponse;
            }

            var listOfNotAvailableTimes = await _dbContext.Appointments.Where(a => a.Date.Date == selectedDate.Date).Select(a => a.Time).ToListAsync();

            List<string> allTimes = new List<string> { "8:00", "9:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00" };


            foreach (string time in listOfNotAvailableTimes)
            {
                if (allTimes.Contains(time))
                {
                    allTimes.Remove(time);
                }
            }

            _apiResponse.Result = allTimes;
            _apiResponse.IsSuccess = true;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            return _apiResponse;
        }
    }
}
