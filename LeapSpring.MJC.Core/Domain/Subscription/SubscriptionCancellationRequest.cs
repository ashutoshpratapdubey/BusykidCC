using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using System;

namespace LeapSpring.MJC.Core.Domain.Subscription
{
    public class SubscriptionCancellationRequest : BaseEntity
    {
        /// <summary>
        /// Gets or sets the family member identifier.
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the requested date.
        /// </summary>
        public DateTime RequestedOn { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction identifier.
        /// </summary>
        public int? BankTransactionID { get; set; }

        /// <summary>
        /// Gets or sets the family member.
        /// </summary>
        public virtual FamilyMember FamilyMember { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction.
        /// </summary>
        public virtual BankTransaction BankTransaction { get; set; }
    }
}
