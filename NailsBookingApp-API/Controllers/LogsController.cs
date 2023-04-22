using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NailsBookingApp_API.Models;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public LogsController(AppDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse>> ClearLogs()
        {
            return null;
        }
    }
}
