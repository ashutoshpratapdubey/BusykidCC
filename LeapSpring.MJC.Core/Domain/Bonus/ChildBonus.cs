using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using System;

namespace LeapSpring.MJC.Core.Domain.Bonus
{
    public class ChildBonus : BaseEntity
    {
        /// <summary>
        /// Gets or sets the bonus amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the child identifier.
        /// </summary>
        public int ChildID { get; set; }

        /// <summary>
        /// Gets or sets the contributor identifier.
        /// </summary>
        public int ContributorID { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction identifier
        /// </summary>
        public int? BankTransactionID { get; set; }

        /// <summary>
        /// Gets or sets the family member
        /// </summary>
        public virtual FamilyMember Child { get; set; }

        /// <summary>
        /// Gets or sets the family member
        /// </summary>
        public virtual FamilyMember Contributor { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction
        /// </summary>
        public virtual BankTransaction BankTransaction { get; set; }
    }
}
