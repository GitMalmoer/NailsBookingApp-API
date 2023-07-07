using System.Net;
using Domain.Models;
using Domain.Models.BOOKING;
using MediatR;
using Microsoft.Extensions.Configuration;
using NailsBookingApp_API.Services;
using Stripe;

namespace Application.MediatR.Booking.Commands
{
    public record InitiatePaymentCommand(CreateAppointmentDTO createAppointmentDto) : IRequest<ApiResponse>;

    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, ApiResponse>
    {
        private readonly IConfiguration _configuration;

        private ApiResponse _apiResponse;
        public InitiatePaymentCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;

            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            List<string> allTimes = new List<string> { "8:00", "9:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00" };

            if (!allTimes.Contains(request.createAppointmentDto.Time))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("Wrong time the you can pick only time from this list: 8:00, 9:00, 11:00, 12:00, 13:00, 14:00, 15:00");
                return _apiResponse;
            }

            bool isDateOk = DateTime.TryParse(request.createAppointmentDto.Date, out DateTime selectedDate);
            if (!isDateOk)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Wrong date");
                return _apiResponse;
            }

            // GETTING THE PRICE FROM DICTIONARY
            double servicePrice = ServiceDictionary.GetPriceByService(request.createAppointmentDto.ServiceValue);

            if (servicePrice == 0)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Service not found");
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
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
                Name = request.createAppointmentDto.Name,
                LastName = request.createAppointmentDto.LastName,
                Date = selectedDate,
                Email = request.createAppointmentDto.Email,
                Phone = request.createAppointmentDto.Phone,
                ServiceValue = request.createAppointmentDto.ServiceValue,
                Time = request.createAppointmentDto.Time,
                ClientSecret = response.ClientSecret,
                StripePaymentIntentId = response.Id,
                Price = servicePrice,
            };

            _apiResponse.Result = newAppointment;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return _apiResponse;

        }
    }
}
