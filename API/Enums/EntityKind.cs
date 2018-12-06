using System.ComponentModel;

namespace MCT.RESTAPI.Enums
{
    internal enum EntityKind
    {
        [Description("Request")]
        Request,

        [Description("Submitted")]
        Subject,

        [Description("User")]
        User,

        [Description("File")]
        File,

        [Description("Confirmation")]
        Confirmation
    }
}