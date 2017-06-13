using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Dto
{
    public class TransactionHistory
    {
        public int ChoreID { get; set; }
        /// <summary>
        /// Gets or sets the name of transaction.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the transaction amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the transaction date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the transaction status
        /// </summary>
        public TransactionStatus TransactionStatus { get; set; }

        /// <summary>
        /// Gets or sets the transaction type.
        /// </summary>
        public TransactionHistoryType TransactionHistoryType { get; set; }

        /// <summary>
        /// Gets or sets the Chore Status type
        /// </summary>
        /// Code Added By Abhishek Dated 25-Apr-2017 Adding a field ChrStatus
        public ChoreStatus? ChrStatus { get; set; }

        /// <summary>
        /// Gets or sets the transaction out type
        /// </summary>
        public EarningsBucketType? TransactionOutType { get; set; }

        public bool choreApprovalFlag { get; set; }
        public bool chorePaydayApprovalFlag { get; set; }

    }
}
