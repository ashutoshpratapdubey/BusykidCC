using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Domain.Email
{
    public class EmailHistory : BaseEntity
    {
        /// <summary>
        /// Gets ot sets the family member identifier.
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the family member.
        /// </summary>
        public virtual FamilyMember FamilyMember { get; set; }

        /// <summary>
        /// Gets or sets the email type
        /// </summary>
        public EmailType EmailType { get; set; }

        /// <summary>
        /// Gets or sets the triggered on date.
        /// </summary>
        public DateTime TriggeredOn { get; set; }
    }
}
