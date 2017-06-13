using System.ComponentModel;

namespace LeapSpring.MJC.Core.Enums
{
    public enum SubscriptionStatus
    {
        [Description("no subscription")]
        NoSubscription,

        [Description("active")]
        Active,

        [Description("pending cancellation")]
        PendingCancellation,

        [Description("cancelled")]
        Cancelled,

        [Description("trial expired")]
        TrialExpired,

        [Description("expired")]
        Expired
    }
}
