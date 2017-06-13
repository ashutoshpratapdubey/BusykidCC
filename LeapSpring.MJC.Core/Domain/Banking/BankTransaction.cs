using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Domain.Banking
{
    public class BankTransaction : BaseEntity
    {
        /// <summary>
        /// Gets or sets the family member identifier
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the transaction identifier
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction status
        /// </summary>
        public TransactionStatus TransactionStatus { get; set; }

        /// <summary>
        /// Gets or sets the payment type of the transaction.
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Gets or sets the created time
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the last updated time
        /// </summary>
        public DateTime? LastUpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the family member
        /// </summary>
        public virtual FamilyMember FamilyMember { get; set; }
    }
}
