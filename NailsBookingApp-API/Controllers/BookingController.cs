using System.Net;
using Application.MediatR.Booking.Commands;
using Application.MediatR.Booking.Querries;
using Domain.Models;
using Domain.Models.BOOKING;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailsBookingApp_API.Controllers.Base;
using NailsBookingApp_API.Services;
using Stripe;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ApiControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private ApiResponse _apiResponse;

        public BookingController(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _apiResponse = new ApiResponse();
        }

        [HttpGet("getAvailableTimes")]
        public async Task<IActionResult> GetAvailableTimes(string stringDate)
        {
            var result = await Mediator.Send(new GetAvailableTimesQuerry(stringDate));
            return await HandleResult(result);
        }

        [HttpPost("createAppointment")]
        public async Task<ActionResult<ApiResponse>> CreateAppointment([FromBody] CreateAppointmentDTO createAppointmentDto)
        {
            var result = await Mediator.Send(new CreateAppointmentCommand(createAppointmentDto));
            return await HandleResult(result);
        }

        [HttpPost("initiatePayment")]
        public async Task<ActionResult<ApiResponse>> InitiatePayment([FromBody] CreateAppointmentDTO createAppointmentDto)
        {
            if (!ModelState.IsValid)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("Model state not valid");
                return BadRequest(_apiResponse);
            }

            var result = await Mediator.Send(new InitiatePaymentCommand(createAppointmentDto));
            return await HandleResult(result);
        }

    }
}
