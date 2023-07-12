using Domain.Models;

namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(Message message);

        Task SendEmailVeryficationLink(string emailVerificationLink, string recipent);
        Task SendPasswordResetLink(string passwordResetLink, string recipent);
        Task SendQuestion(string name, string email, string message);
    }
}
