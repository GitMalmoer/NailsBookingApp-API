using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NailsBookingApp_API.Exceptions;
using NailsBookingApp_API.Models;

namespace NailsBookingApp_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly IConfiguration _configuration;
        private string _questionRecipent;

        public EmailService(EmailConfiguration emailConfig, IConfiguration configuration)
        {
            _emailConfig = emailConfig;
            _configuration = configuration;
            _questionRecipent = _configuration.GetValue<string>("QuestionRecipent");
        }

        public async Task SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await Send(emailMessage);
        }


        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email",_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private async Task Send(MimeMessage emailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                    await client.SendAsync(emailMessage);
                }
                catch (Exception e)
                {
                    var exceptionDetails = e.ToString();
                    throw new EmailErrorException($"Error during sending email. Date:{DateTime.Now}, \n" + exceptionDetails);
                }

            }
        }

        public async Task SendEmailVeryficationLink(string emailVerificationLink, string recipent)
        {
            List<string> listOfRecipents = new List<string>
            {
                recipent
            };

            string subject = "Email Verification Link";

            string message = $"Here is your email verification link: {emailVerificationLink}";

            Message msg = new Message(listOfRecipents, subject, message);

            await SendEmail(msg);
        }

        public async Task SendPasswordResetLink(string passwordResetLink, string recipent)
        {
            List<string> listOfRecipents = new List<string>
            {
                recipent
            };

            string subject = "Password Reset";


            string message = $"Here is your password reset link: {passwordResetLink}";

            Message msg = new Message(listOfRecipents, subject, message);

            await SendEmail(msg);
        }

        public async Task SendQuestion(string name, string email, string message)
        {
            List<string> listOfRecipents = new List<string>
            {
                _questionRecipent,
            };

            string subject = "Question From Nails Website";

            string combinedMessage = $"{name} \n {email} \n wrote a question to you: \n {message}";

            Message msg = new Message(listOfRecipents, subject, combinedMessage);

            await SendEmail(msg);
        }
    }
}
