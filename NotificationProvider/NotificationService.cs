using Microsoft.Extensions.Configuration;
using NotificationProvider.Interfaces;
using NotificationProvider.Models;
using NotificationProvider.Services;
using System.Net.Mail;

namespace NotificationProvider
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration configuration;
        private readonly Mailer mailer;

        public NotificationService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.mailer = new Mailer(configuration["SendGrid:Endpoint"], configuration["SendGrid:ApiKey"]);
        }

        public async void PushAsync(Notification notification)
        {
            SendGridMail mail = new SendGridMail()
            {
                To = new MailAddress(notification.Recipient),
                Subject = notification.Subject,
                Content = notification.Message
            };

            await mailer.Send(mail);
        }

        public bool SendSlackMessage(string user, string mesage)
        {
            return true;
        }
    }
}