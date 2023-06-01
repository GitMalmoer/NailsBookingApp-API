using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailsBookingApp_API.Models;
using NailsBookingApp_API.Models.BOOKING;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public BookingController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }

        [HttpGet("GetAvailableTimes")]
        public async Task<IActionResult> GetAvailableTimes(DateTime selectedDate)
        {
           var listOfNotAvailableTimes = await _dbContext.Appointments.Where(a => a.Date.Date == selectedDate.Date).Select(a => a.Time).ToListAsync();

           List<string> allTimes = new List<string>{ "8:00", "9:00", "10:00", "11:00", "12:00", "13:00", "14:00","15:00"};


           foreach (string time in listOfNotAvailableTimes)
           {
               if(allTimes.Contains(time))
               {
                   allTimes.Remove(time);
               }
           }

           _apiResponse.Result = allTimes;
           _apiResponse.IsSuccess = true;
           _apiResponse.HttpStatusCode = HttpStatusCode.OK;
           return Ok(_apiResponse);
        }

        [HttpPost("CreateAppointment")]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentDTO createAppointmentDto)
        {
            if (!ModelState.IsValid)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("Model state not valid");
                return BadRequest(_apiResponse);
            }

            List<string> allTimes = new List<string> { "8:00", "9:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00" };

            if (!allTimes.Contains(createAppointmentDto.Time))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("Wrong time the you can pick only time from this list: 8:00, 9:00, 11:00, 12:00, 13:00, 14:00, 15:00");
                return BadRequest(_apiResponse);
            }

            Appointment newAppointment = new Appointment()
            {
                Name = createAppointmentDto.Name,
                LastName = createAppointmentDto.LastName,
                Date = createAppointmentDto.Date,
                Email = createAppointmentDto.Email,
                Phone = createAppointmentDto.Phone,
                Price = createAppointmentDto.Price,
                ServiceValue = createAppointmentDto.ServiceValue,
                Time = createAppointmentDto.Time,
            };

            await _dbContext.Appointments.AddAsync(newAppointment);
            await _dbContext.SaveChangesAsync();
            _apiResponse.Result = newAppointment;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);

        }

    }
}
