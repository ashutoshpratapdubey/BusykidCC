using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Domain.Save
{
    public class StockPurchaseRequest : BaseEntity
    {
        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        public Guid TransactionID { get; set; }

        /// <summary>
        /// Gets or sets the line item identifier.
        /// </summary>
        public Guid LineItemID { get; set; }

        /// <summary>
        /// Gets or sets the stock item identifier.
        /// </summary>
        public int StockItemID { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the stock price at that time of purchase.
        /// </summary>
        public decimal StockPrice { get; set; }

        /// <summary>
        /// Gets or sets the stock purchase status.
        /// </summary>
        public ApprovalStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the child identifier.
        /// </summary>
        public int ChildID { get; set; }

        /// <summary>
        /// Gets or sets the purchased date.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction identifier.
        /// </summary>
        public int? BankTransactionID { get; set; }

        /// <summary>
        /// Gets or sets the fee amount.
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        public virtual StockItem StockItem { get; set; }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        public virtual FamilyMember Child { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction.
        /// </summary>
        public virtual BankTransaction BankTransaction { get; set; }
    }
}
