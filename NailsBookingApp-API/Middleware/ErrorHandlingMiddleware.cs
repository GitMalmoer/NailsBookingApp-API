using System.Net;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NailsBookingApp_API.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (EmailErrorException e)
            {
                // WHEN ERROR OCCURS ITS AUTO LOGGED IN DATABASE
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                ProblemDetails problem = new ProblemDetails()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Type = "Server Error while sending email",
                    Title = "Server Error while sending email",
                    Detail = "An Internal Server Error Has Occured during sending SMTP email",
                };

                var json = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.ToString());

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                ProblemDetails problem = new ProblemDetails()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Type = "Server Error",
                    Title = "Server Error",
                    Detail = "An Internal Server Error Has Occured",
                };

                var json = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }
            finally
            {
                // LOG INTO DATABASE FINALLY

                //_logger.LogDebug("This is a debug message");
                //_logger.LogInformation("This is an info message");
                //_logger.LogWarning("This is a warning message ");
                //_logger.LogError(new Exception(), "This is an error message");
            }
        }
    }
}
