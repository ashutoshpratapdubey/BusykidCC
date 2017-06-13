using LeapSpring.MJC.Core.Enums;

namespace LeapSpring.MJC.Core.Dto
{
    public class Subscription
    {
        /// <summary>
        /// Gets or sets the subscription type.
        /// </summary>
        public SubscriptionType SubscriptionType { get; set; }

        /// <summary>
        /// Gets or sets the subscription promo code.
        /// </summary>
        public string PromoCode { get; set; }
    }
}
