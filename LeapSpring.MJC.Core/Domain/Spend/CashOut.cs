using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Domain.Spend
{
    public class CashOut : BaseEntity
    {
        /// <summary>
        /// Gets or sets the child identifier.
        /// </summary>
        public int ChildID { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction identifier.
        /// </summary>
        public int? BankTransactionID { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the approval status
        /// </summary>
        public ApprovalStatus ApprovalStatus { get; set; }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        public virtual FamilyMember Child { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction.
        /// </summary>
        public virtual BankTransaction BankTransaction { get; set;}
    }
}
