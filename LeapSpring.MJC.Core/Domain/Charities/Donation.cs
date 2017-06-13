using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Domain.Charities
{
    public class Donation : BaseEntity
    {
        /// <summary>
        /// Gets or sets the charity identifier.
        /// </summary>
        public int CharityID { get; set; }

        /// <summary>
        /// Gets or sets the family member identifier.
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the amount to be donated.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the approval status
        /// </summary>
        public ApprovalStatus ApprovalStatus { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction identifier.
        /// </summary>
        public int? BankTransactionID { get; set; }

        /// <summary>
        /// Gets or sets the charity.
        /// </summary>
        public virtual Charity Charity { get; set; }

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
