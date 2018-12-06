namespace NotificationProvider.Models.Notifications
{
    public class EmailNotification : Notification
    {
        public EmailNotification(string recipient, string message) : base(recipient, message)
        {
        }
    }
}