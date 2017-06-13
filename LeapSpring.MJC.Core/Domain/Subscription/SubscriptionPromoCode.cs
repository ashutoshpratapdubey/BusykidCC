using LeapSpring.MJC.Core.Enums;

namespace LeapSpring.MJC.Core.Domain.Subscription
{
    public class SubscriptionPromoCode : BaseEntity
    {
        /// <summary>
        /// Gets or sets the promo code.
        /// </summary>
        public string PromoCode { get; set; }

        /// <summary>
        /// Gets or sets the duration of the promo code in months.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the duration type.
        /// </summary>
        public DurationType DurationType { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the is Pixelscript.
        /// </summary>
        public string PixelScript { get; set; }



    }
}
