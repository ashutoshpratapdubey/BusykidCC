using LeapSpring.MJC.Core.Domain.Family;
using System;

namespace LeapSpring.MJC.Core.Domain.Banking
{
    /// <summary>
    /// Represents a transaction log
    /// </summary>
    public class TransactionLog : BaseEntity
    {
        /// <summary>
        /// Gets or sets the family member identifier
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the family member
        /// </summary>
        public virtual FamilyMember FamilyMember { get; set; }
    }
}
