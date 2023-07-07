using Application.MediatR.Logging;
using Domain.Models;
using Domain.Utility;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NailsBookingApp_API.Controllers.Base;

namespace NailsBookingApp_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = SD.Role_Admin)]
    public class LogsController : ApiControllerBase
    {
        [HttpDelete("clearLogs")]
        public async Task<ActionResult<ApiResponse>> ClearLogs()
        {
            var result = await Mediator.Send(new ClearLogsCommand());
            return await HandleResult(result);
        }

        [HttpGet("getErrorLogs")]
        public async Task<ActionResult<ApiResponse>> GetErrorLogs()
        {
            var result = await Mediator.Send(new GetErrorLogsQuerry());
            return await HandleResult(result);
        }
    }
}
