namespace NotificationProvider.Models
{
    public class Notification
    {
        internal string Recipient { get; set; }

        internal string Message { get; set; }

        internal string Subject { get; set; }

        public Notification(string recipient, string subject, string message)
        {
            this.Recipient = recipient;
            this.Subject = subject;
            this.Message = message;
        }
    }
}