namespace NotificationProvider.Models.Notifications
{
    public class EmailNotification : Notification
    {
        public EmailNotification(string recipient, string subject, string message) : base(recipient, subject, message)
        {
        }
    }
}