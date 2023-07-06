using System.Net;
using Application.DTO;
using Domain.Models;
using Domain.Utility;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NailsBookingApp_API.Services;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailMessageController : ControllerBase
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
            if (!ModelState.IsValid)
            {
                _apiResponse.ErrorMessages.Add("ModelState Not valid");
                _apiResponse.IsSuccess = false;
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            await _emailService.SendQuestion(emailQuestionDto.Name, emailQuestionDto.Email, emailQuestionDto.Message);

            _dbContext.EmailQuestions.Add(new EmailQuestion()
            {
                Email = emailQuestionDto.Email,
                Name = emailQuestionDto.Name,
                Message = emailQuestionDto.Message,
            });
            await _dbContext.SaveChangesAsync();

            _apiResponse.IsSuccess = true;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet("getMessages")]
        public async Task<ActionResult<ApiResponse>> GetMessages()
        {
            var emailMessages = _dbContext.EmailQuestions;
            _logger.LogInformation("Just a test log");

            if (emailMessages.Any())
            {
                _apiResponse.IsSuccess = true;
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.Result = emailMessages;
                return Ok(_apiResponse);
            }


            _apiResponse.IsSuccess = false;
            _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
            _apiResponse.ErrorMessages.Add("No messages found");
            return NotFound(_apiResponse);
        }

    }
}
