using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Domain.Spend
{
    /// <summary>
    /// Represents a purchased gift card
    /// </summary>
    public class PurchasedGiftCard : BaseEntity
    {
        /// <summary>
        /// Gets or sets the family member identifier
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the card identifier
        /// </summary>
        public int CardId { get; set; }

        /// <summary>
        /// Gets or sets the merchant name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the cover image url
        /// </summary>
        public string CoverImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the purchased gift card url
        /// </summary>
        public string GiftCardUrl { get; set; }

        /// <summary>
        /// Gets or sets the purchased date
        /// </summary>
        public DateTime PurchasedOn { get; set; }

        /// <summary>
        /// Gets or sets the approval status
        /// </summary>
        public ApprovalStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether deleted
        /// </summary>
        /// <value>
        /// <c>true</c> if deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction identifier.
        /// </summary>
        public int? BankTransactionID { get; set; }

        /// <summary>
        /// Gets or sets the family member
        /// </summary>
        public virtual FamilyMember FamilyMember { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction.
        /// </summary>
        public virtual BankTransaction BankTransaction { get; set; }
    }
}
