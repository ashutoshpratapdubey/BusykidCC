using LeapSpring.MJC.Core.Domain.Earnings;
using LeapSpring.MJC.Core.Domain.Settings;
using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Dto
{
    public class ChildFinancialOverview
    {
        /// <summary>
        /// Gets or sets the next pay day.
        /// </summary>
        public string NextPayDate { get; set; }

        /// <summary>
        /// Gets or sets the next pay amount.
        /// </summary>
        public decimal NextPayAmount { get; set; }

        /// <summary>
        /// Gets or sets the remaining days for next pay day.
        /// </summary>
        public int RemainingDays { get; set; }

        /// <summary>
        /// Gets or sets the allocation settings.
        /// </summary>
        public AllocationSettings AllocationSettings { get; set; }

        /// <summary>
        /// Gets or sets the child earnings.
        /// </summary>
        public ChildEarnings Earnings { get; set; }

        /// <summary>
        /// Gets or sets the next pay distribution.
        /// </summary>
        public ChildEarnings NextPayDistribution { get; set; }

        /// <summary>
        /// Change the UI design as per status[STPL]
        /// </summary>
        public string choreStatus { get; set; }
        public decimal pendingPaydayAmount { get; set; }

        public bool pendingChoreStatus { get; set; }

        public bool childChoreStatus { get; set; }
    }
    public class childApprovalDetails
    {
        public ChoreStatus ApprovalStatus { get; set; }
        public decimal Amount { get; set; }

    }
}
