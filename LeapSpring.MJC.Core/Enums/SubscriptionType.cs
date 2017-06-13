using System.ComponentModel;

namespace LeapSpring.MJC.Core.Enums
{
    public enum SubscriptionType
    {
        [Description("Annual")]
        Annual,

        [Description("One month free trial")]
        OneMonthTrial,

        [Description("Promo plan")]
        PromoPlan,

        [Description("Pending microtransaction account")]
        Pendingaccount

    }
}
