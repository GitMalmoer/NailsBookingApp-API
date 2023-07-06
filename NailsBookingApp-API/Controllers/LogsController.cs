using Domain.Models;
using Domain.Utility;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_Admin)]
    public class LogsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public LogsController(AppDbContext _dbContext)
        {
            this._dbContext = _dbContext;
            _apiResponse = new ApiResponse();
        }

        [HttpDelete("clearLogs")]
        public async Task<ActionResult<ApiResponse>> ClearLogs()
        {
            var logs = _dbContext.Logs;
            _dbContext.RemoveRange(logs);
            await _dbContext.SaveChangesAsync();

            return Ok("Removed");
        }

        [HttpGet("getErrorLogs")]
        public async Task<ActionResult<ApiResponse>> GetErrorLogs()
        {

            //var userId = User.FindFirst("Id")?.Value;

            var logs = _dbContext.Logs.Where(l => l.Level == "error");
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = logs;
            return Ok(_apiResponse);
        }
    }
}
