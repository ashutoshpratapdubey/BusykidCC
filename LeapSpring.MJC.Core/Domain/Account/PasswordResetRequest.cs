using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Domain.Family;
using System;

namespace LeapSpring.MJC.Core.Domain.Account
{
    public class PasswordResetRequest : BaseEntity
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
        /// Gets ot sets the token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Gets or sets the password reset status.
        /// </summary>
        public PasswordResetStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}
