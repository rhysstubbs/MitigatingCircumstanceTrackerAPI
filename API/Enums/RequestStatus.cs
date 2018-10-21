using System.ComponentModel;

namespace MCT.RESTAPI.Enums
{
    internal enum RequestStatus
    {
        [Description("Submitted")]
        Submitted,

        [Description("InReview")]
        InReview,

        [Description("Approved)")]
        Approved,

        [Description("Archived")]
        Archived
    }
}