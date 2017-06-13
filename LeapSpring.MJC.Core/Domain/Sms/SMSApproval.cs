using LeapSpring.MJC.Core.Domain.Charities;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Save;
using LeapSpring.MJC.Core.Domain.Spend;
using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Domain.Sms
{
    public class SMSApproval : BaseEntity
    {
        /// <summary>
        /// Gets or sets the approval request type.
        /// </summary>
        public ApprovalType ApprovalType { get; set; }

        /// <summary>
        /// Gets or sets the admin member identifier of the family.
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the last sent on date.
        /// </summary>
        public DateTime LastSentOn { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the purchased gift card identifier.
        /// </summary>
        public int? PurchasedGiftCardID { get; set; }

        /// <summary>
        /// Gets or sets the stock purchase request identifier.
        /// </summary>
        public int? StockPurchaseRequestID { get; set; }

        /// <summary>
        /// Gets or sets the donation identifier.
        /// </summary>
        public int? DonationID { get; set; }

        /// <summary>
        /// Gets or sets the cashout identifier.
        /// </summary>
        public int? CashOutID { get; set; }

        /// <summary>
        /// Get or sets the is reminded.
        /// </summary>
        public bool IsReminded { get; set; }

        /// <summary>
        /// Gets or sets the purchased gift card.
        /// </summary>
        public PurchasedGiftCard PurchasedGiftCard { get; set; }

        /// <summary>
        /// Gets or sets the stock purchase request.
        /// </summary>
        public StockPurchaseRequest StockPurchaseRequest { get; set; }

        /// <summary>
        /// Gets or sets the donation.
        /// </summary>
        public Donation Donation { get; set; }

        /// <summary>
        /// Gets or sets the cashout.
        /// </summary>
        public CashOut CashOut { get; set; }

        /// <summary>
        /// Gets or sets the family member.
        /// </summary>
        public FamilyMember FamilyMember { get; set; }
        public bool responseStatus { get; set; }
    }
}
