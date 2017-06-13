using System.ComponentModel;

namespace LeapSpring.MJC.Core.Enums
{
    public enum PaymentType
    {
        [Description("chore")]
        Chore = 1,

        [Description("bonus")]
        Bonus = 2,

        [Description("charity")]
        Charity = 3,

        [Description("cashout")]
        CashOut = 4,

        [Description("family subscription")]
        Subscription = 5,

        [Description("family subscription cancellation")]
        SubscriptionCancellation = 6,

        [Description("Child deletion")]
        ChildRemoved = 7,

        [Description("Stock pile")]
        StockPile = 8,

        [Description("Gift card")]
        GiftCard = 9,

        [Description("family subscription transaction initiated")]
        SubscriptionInitiated = 10,

    }
}
