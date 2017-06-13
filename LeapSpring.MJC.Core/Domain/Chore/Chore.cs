using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeapSpring.MJC.Core.Domain.Chore
{
    public class Chore : BaseEntity
    {
        /// <summary>
        /// Gets or sets the chore name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of chore.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the chore image url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets is completed. <c>True</c>, if chore has been completed. <c>False</c>, otherwise.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets the system chore identifier.
        /// </summary>
        public int? SystemChoreID { get; set; }

        /// <summary>
        /// Gets or sets the frequency type.
        /// </summary>
        public FrequencyType FrequencyType { get; set; }

        /// <summary>
        /// Gets or sets the frequency range.
        /// </summary>
        public string FrequencyRange { get; set; }

        /// <summary>
        /// Gets or sets the created time.
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the chore status.
        /// </summary>
        public ChoreStatus ChoreStatus { get; set; }

        /// <summary>
        /// Gets or sets the recurring chore identifier.
        /// </summary>
        public int? RecurringChoreID { get; set; }

        /// <summary>
        /// Gets or sets the chore completion date.
        /// </summary>
        public DateTime? CompletedOn { get; set; }

        /// <summary>
        /// Gets or sets the chore disapproved date
        /// </summary>
        public DateTime? DisapprovedOn { get; set; }

        /// <summary>
        /// Gets or sets the is delete.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the family member identifier.
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction identifier.
        /// </summary>
        public int? BankTransactionID { get; set; }

        /// <summary>
        /// Gets or sets the family member.
        /// </summary>
        public virtual FamilyMember FamilyMember { get; set; }

        /// <summary>
        /// Gets or sets the bank transaction.
        /// </summary>
        public virtual BankTransaction BankTransaction { get; set; }

        /// <summary>
        /// Gets or sets if system chore updated
        /// </summary>
        [NotMapped]
        public bool IsSystemChoreUpdated { set; get; }

        public DateTime? RemoveAprovalDate { get; set; }

        public DateTime? ApprovedApprovalDate { get; set; }
        public bool IncludedFlag { get; set; }
    }
}
