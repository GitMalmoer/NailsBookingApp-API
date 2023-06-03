using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailsBookingApp_API.Models;
using NailsBookingApp_API.Models.BOOKING;
using NailsBookingApp_API.Services;
using Stripe;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
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

        [HttpGet("GetAvailableTimes")]
        public async Task<IActionResult> GetAvailableTimes(string stringDate)
        {
            bool isDateOk = DateTime.TryParse(stringDate, out DateTime selectedDate);

            if (!isDateOk)
            {
                _apiResponse.ErrorMessages.Add("Wrong date");
                _apiResponse.IsSuccess = false;
                return BadRequest(_apiResponse);
            }

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
        public async Task<ActionResult<ApiResponse>> CreateAppointment([FromBody]CreateAppointmentDTO createAppointmentDto)
        {
            try
            {
                   bool isDateOk = DateTime.TryParse(createAppointmentDto.Date, out DateTime selectedDate);
            if (!isDateOk)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Wrong date");
                return BadRequest(_apiResponse);
            }

            //double price;
            //Double.TryParse(createAppointmentDto.Price, out price);

            Appointment newAppointment = new Appointment()
            {
                Name = createAppointmentDto.Name,
                LastName = createAppointmentDto.LastName,
                Date = selectedDate,
                Email = createAppointmentDto.Email,
                Phone = createAppointmentDto.Phone,
                ServiceValue = createAppointmentDto.ServiceValue,
                Time = createAppointmentDto.Time,
                Price = createAppointmentDto.Price ?? 0,
            };

            await _dbContext.Appointments.AddAsync(newAppointment);
            await _dbContext.SaveChangesAsync();
            _apiResponse.Result = newAppointment;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
            }
            catch (Exception e)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Error");
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }
        }

        [HttpPost("InitiatePayment")]
        public async Task<ActionResult<ApiResponse>> InitiatePayment([FromBody] CreateAppointmentDTO createAppointmentDto)
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

            bool isDateOk = DateTime.TryParse(createAppointmentDto.Date, out DateTime selectedDate);
            if (!isDateOk)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Wrong date");
                return BadRequest(_apiResponse);
            }

            // GETTING THE PRICE FROM DICTIONARY
            double servicePrice = ServiceDictionary.GetPriceByService(createAppointmentDto.ServiceValue);
            
            if (servicePrice == 0)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Service not found");
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }


            #region Create Payment Intent
            StripeConfiguration.ApiKey = _configuration.GetValue<string>("StripeSettings:SecretKey");
            PaymentIntentCreateOptions options = new PaymentIntentCreateOptions()
            {
                Amount = (int)(servicePrice * 100),
                Currency = "SEK",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions()
                {
                    Enabled = true
                }
            };

            PaymentIntentService service = new PaymentIntentService();
            PaymentIntent response = await service.CreateAsync(options);

            #endregion

            Appointment newAppointment = new Appointment()
            {
                Name = createAppointmentDto.Name,
                LastName = createAppointmentDto.LastName,
                Date = selectedDate,
                Email = createAppointmentDto.Email,
                Phone = createAppointmentDto.Phone,
                ServiceValue = createAppointmentDto.ServiceValue,
                Time = createAppointmentDto.Time,
                ClientSecret = response.ClientSecret,
                StripePaymentIntentId = response.Id,
                Price = servicePrice,
            };

            _apiResponse.Result = newAppointment;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);

        }

    }
}
