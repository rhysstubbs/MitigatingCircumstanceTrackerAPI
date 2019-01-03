using System.ComponentModel;

namespace MCT.RESTAPI.Enums
{
    public enum RequestStatus
    {
        [Description("Orphaned")]
        Orphaned,

        [Description("Submitted")]
        Submitted,

        [Description("InReview")]
        InReview,

        [Description("Approved)")]
        Approved,

        [Description("Denied")]
        Denied,

        [Description("Archived")]
        Archived,

        [Description("MoreInfoRequired")]
        MoreInfoRequired,

        [Description("Updated")]
        Updated
    }
}