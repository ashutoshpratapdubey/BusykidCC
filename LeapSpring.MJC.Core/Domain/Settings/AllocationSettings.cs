using LeapSpring.MJC.Core.Domain.Family;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Domain.Settings
{
    /// <summary>
    /// Represents a child money allocation settings
    /// </summary>
    public class AllocationSettings : BaseEntity
    {
        /// <summary>
        /// Gets or sets the family member identifier
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the save
        /// </summary>
        public decimal Save { get; set; }

        /// <summary>
        /// Gets or sets the share
        /// </summary>
        public decimal Share { get; set; }

        /// <summary>
        /// Gets or sets the spend
        /// </summary>
        public decimal Spend { get; set; }

        /// <summary>
        /// Gets or sets the family member
        /// </summary>
        public virtual FamilyMember FamilyMember { get; set; }
    }
}
