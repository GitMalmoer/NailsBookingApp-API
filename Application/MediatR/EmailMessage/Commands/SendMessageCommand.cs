using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.DTO;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatR.EmailMessage.Commands
{
    public record SendMessageCommand(EmailQuestionDTO emailQuestionDto) : IRequest<ApiResponse>;

    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, ApiResponse>
    {
        private readonly IEmailService _emailService;
        private readonly IAppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public SendMessageCommandHandler(IEmailService emailService, IAppDbContext dbContext)
        {
            _emailService = emailService;
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }
        public async Task<ApiResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            await _emailService.SendQuestion(request.emailQuestionDto.Name, request.emailQuestionDto.Email, request.emailQuestionDto.Message);

            _dbContext.EmailQuestions.Add(new EmailQuestion()
            {
                Email = request.emailQuestionDto.Email,
                Name = request.emailQuestionDto.Name,
                Message = request.emailQuestionDto.Message,
            });
            await _dbContext.SaveChangesAsync(cancellationToken);

            _apiResponse.IsSuccess = true;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            return _apiResponse;
        }
    }
}
