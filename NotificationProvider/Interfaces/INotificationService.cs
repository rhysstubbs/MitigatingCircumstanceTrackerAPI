using NotificationProvider.Models;

namespace NotificationProvider.Interfaces
{
    public interface INotificationService
    {
        bool PushAsync(Notification notification);

        bool SendSlackMessage(string user, string message);
    }
}