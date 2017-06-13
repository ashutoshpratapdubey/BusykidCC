using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Domain.Subscription
{
    public class FamilySubscription : BaseEntity
    {
        /// <summary>
        /// Gets or sets the subscription plan identifier.
        /// </summary>
        public int SubscriptionPlanID { get; set; }

        /// <summary>
        /// Gets or sets the starts on date
        /// </summary>
        public DateTime StartsOn { get; set; }

        /// <summary>
        /// Gets or sets the ends on date
        /// </summary>
        public DateTime EndsOn { get; set; }

        /// <summary>
        /// Gets or sets the subscription status
        /// </summary>
        public SubscriptionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the promo code.
        /// </summary>
        public string PromoCode { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction identifier.
        /// </summary>
        public int? BankTransactionID { get; set; }

        /// <summary>
        /// Gets or sets the is trial used.
        /// </summary>
        public bool IsTrialUsed { get; set; }

        /// <summary>
        /// Gets or sets the trial start date.
        /// </summary>
        public DateTime? TrialStartDate { get; set; }

        /// <summary>
        /// Gets or sets the subscription plan.
        /// </summary>
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction.
        /// </summary>
        public virtual BankTransaction BankTransaction { get; set; }
    }
}
