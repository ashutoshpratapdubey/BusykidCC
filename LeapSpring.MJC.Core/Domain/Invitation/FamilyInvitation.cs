using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Domain.Family;
using System;

namespace LeapSpring.MJC.Core.Domain.Invitation
{
    public class FamilyInvitation : BaseEntity
    {
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Gets ot sets the token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Gets or sets the member type.
        /// </summary>
        public MemberType MemberType { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the invitation status.
        /// </summary>
        public InvitationStatus Status { get; set; }

        /// <summary>
        /// Gets ot sets the family identifier.
        /// </summary>
        public int FamilyID { get; set; }

        /// <summary>
        /// Gets or sets the family.
        /// </summary>
        public virtual Family.Family Family { get; set; }
    }
}
