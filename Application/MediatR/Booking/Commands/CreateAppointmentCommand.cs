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
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.Booking.Commands
{
    public record CreateAppointmentCommand(CreateAppointmentDTO createAppointmentDto) : IRequest<ApiResponse>;

    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand,ApiResponse>
    {
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;
        public CreateAppointmentCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool isDateOk = DateTime.TryParse(request.createAppointmentDto.Date, out DateTime selectedDate);
                if (!isDateOk)
                {
                    _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("Wrong date");
                    return _apiResponse;
                }

                //double price;
                //Double.TryParse(createAppointmentDto.Price, out price);

                Appointment newAppointment = new Appointment()
                {
                    Name = request.createAppointmentDto.Name,
                    LastName = request.createAppointmentDto.LastName,
                    Date = selectedDate,
                    Email = request.createAppointmentDto.Email,
                    Phone = request.createAppointmentDto.Phone,
                    ServiceValue = request.createAppointmentDto.ServiceValue,
                    Time = request.createAppointmentDto.Time,
                    Price = request.createAppointmentDto.Price ?? 0,
                };

                await _dbContext.Appointments.AddAsync(newAppointment);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _apiResponse.Result = newAppointment;
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return _apiResponse;
            }
            catch (Exception e)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Error");
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
            }
        }
    }

}
