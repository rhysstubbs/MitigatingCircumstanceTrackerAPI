using System.ComponentModel;

namespace NotificationProvider.Enums
{
    public enum NotificationDefaults
    {
        [Description("Email Notification")]
        SuccessSubmission,

        [Description("Slack Message")]
        Denied,

        [Description("Trello Update")]
        Trello
    }
}