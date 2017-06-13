using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Domain.Sms
{
    /// <summary>
    /// Represents a add chore sms response workflow
    /// </summary>
    public class ChoreWorkflow : BaseEntity
    {
        /// <summary>
        /// Gets or sets the family member identifier
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the child family member identifier
        /// </summary>
        public int ChildMemberID { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public decimal? Value { get; set; }

        /// <summary>
        /// Gets or sets the frequency type
        /// </summary>
        public FrequencyType? FrequencyType { get; set; }

        /// <summary>
        /// Gets or sets the frequency range
        /// </summary>
        public string FrequencyRange { get; set; }

        /// <summary>
        /// Gets or sets the workflow status
        /// </summary>
        public WorkflowStatus WorkflowStatus { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the family member
        /// </summary>
        public virtual FamilyMember FamilyMember { get; set; }

        /// <summary>
        /// Gets or sets the child family member
        /// </summary>
        public virtual FamilyMember ChildMember { get; set; }
    }
}
