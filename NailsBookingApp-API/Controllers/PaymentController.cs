using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private ApiResponse _apiResponse;
        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiResponse = new ApiResponse();
        }
    }
}
