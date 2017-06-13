namespace LeapSpring.MJC.Core.Domain.Subscription
{
    public class SubscriptionPlan : BaseEntity
    {
        /// <summary>
        /// Gets or sets the plan name.
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }

    }
}
