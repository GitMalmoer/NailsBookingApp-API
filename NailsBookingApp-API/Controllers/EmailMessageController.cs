using System.Net;
using Application.Common.Interfaces;
using Application.DTO;
using Application.MediatR.EmailMessage.Commands;
using Application.MediatR.EmailMessage.Querries;
using Domain.Models;
using Domain.Utility;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NailsBookingApp_API.Controllers.Base;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailMessageController : ApiControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailMessageController> _logger;
        private ApiResponse _apiResponse;


        public EmailMessageController(AppDbContext dbContext, IEmailService emailService, ILogger<EmailMessageController> logger)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _logger = logger;
            _apiResponse = new ApiResponse();
        }

        [HttpPost("sendMessage")]
        public async Task<ActionResult<ApiResponse>> AskQuestion([FromBody] EmailQuestionDTO emailQuestionDto)
        {
            var result = await Mediator.Send(new SendMessageCommand(emailQuestionDto));
            return await HandleResult(result);
        }

        [HttpGet("getMessages")]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<ActionResult<ApiResponse>> GetMessages()
        {
            var result = await Mediator.Send(new GetMessagesQuerry());
            return await HandleResult(result);
        }

    }
}
