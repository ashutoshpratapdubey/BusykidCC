using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Domain.Account
{
    public class PhoneNumberConfirmation : BaseEntity
    {
        /// <summary>
        /// Gets or sets the family member identifier.
        /// </summary>
        public int FamilyMemberID { get; set; }

        /// <summary>
        /// Gets or sets the Verification Code.
        /// </summary>
        public string VerificationCode { get; set; }

        /// <summary>
        /// Gets or sets the created time.
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the OTP Status.
        /// </summary>
        public VerificationCodeStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the family member.
        /// </summary>
        public virtual FamilyMember FamilyMember { get; set; }
    }
}
