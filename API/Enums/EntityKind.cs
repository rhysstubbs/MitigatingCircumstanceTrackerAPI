using System.ComponentModel;

namespace MCT.RESTAPI.Enums
{
    internal enum EntityKind
    {
        [Description("Request")]
        Request,

        [Description("User")]
        User,

        [Description("File")]
        File,

        [Description("Confirmation")]
        Confirmation,

        [Description("Notification")]
        Notification
    }
}