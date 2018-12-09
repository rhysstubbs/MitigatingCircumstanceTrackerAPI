using NotificationProvider.Models;

namespace NotificationProvider.Interfaces
{
    public interface INotificationService
    {
        void PushAsync(Notification notification);

        bool SendSlackMessage(string user, string message);
    }
}