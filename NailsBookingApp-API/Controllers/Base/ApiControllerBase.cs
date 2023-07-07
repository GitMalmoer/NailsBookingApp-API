using System.Net;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace NailsBookingApp_API.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected async Task<ActionResult> HandleResult(ApiResponse apiResponse)
        {
            if (apiResponse == null)
            {
                return NotFound("NULL API RESPONSE ERROR");
            }

            if (apiResponse.HttpStatusCode == default)
            {
                return BadRequest("No HTTP Status Code assigned contact admin");
            }

            if (apiResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                return Ok(apiResponse);
            }

            if (apiResponse.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(apiResponse);
            }

            if (apiResponse.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(apiResponse);
            }

            if (apiResponse.HttpStatusCode == HttpStatusCode.NoContent)
            {
                return NoContent();
            }

            return BadRequest("HANDLE RESULT ERROR");
        }

    }
}
