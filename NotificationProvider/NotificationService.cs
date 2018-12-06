using NotificationProvider.Interfaces;
using NotificationProvider.Models;
using NotificationProvider.Services;
using System.Net.Mail;

namespace NotificationProvider
{
    public class NotificationService : INotificationService
    {
        public bool PushAsync(Notification notification)
        {
            SendGridMail mail = new SendGridMail()
            {
                To = new MailAddress(notification.Recipient),
                Subject = "Test",
                Content = notification.Message
            };

            var mailer = new Mailer();
            var result = mailer.Send(mail);
            if (!result.IsCompletedSuccessfully)
            {
                return false;
            }

            return true;
        }

        public bool SendSlackMessage(string user, string mesage)
        {
            return true;
        }
    }
}