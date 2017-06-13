using LeapSpring.MJC.Core.Domain.Subscription;
using LeapSpring.MJC.Core.Enums;

namespace LeapSpring.MJC.Core.Domain.Family
{
    public class Family : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the family.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unique name of the family.
        /// </summary>
        public string UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the signup status.
        /// </summary>
        public SignUpStatus SignUpStatus { get; set; }

        /// <summary>
        /// Gets or sets the family subscription identifier.
        /// </summary>
        public int? FamilySubscriptionID { get; set; }

        /// <summary>
        /// Gets or sets the family subscription.
        /// </summary>
        public FamilySubscription FamilySubscription { get; set; }

    }
}
