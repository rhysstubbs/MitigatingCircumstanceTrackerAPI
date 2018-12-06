namespace NotificationProvider.Models
{
    public class Notification
    {
        internal string Recipient { get; set; }

        internal string Message { get; set; }

        public Notification(string recipient, string message)
        {
            this.Recipient = recipient;
            this.Message = message;
        }
    }
}