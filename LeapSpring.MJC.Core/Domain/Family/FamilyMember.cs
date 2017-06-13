using LeapSpring.MJC.Core.Enums;
using System;

namespace LeapSpring.MJC.Core.Domain.Family
{
    public class FamilyMember : BaseEntity
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string Lastname { get; set; }

        /// <summary>
        /// Gets or sets the social security number (Last four digit)
        /// </summary>
        public string SSN { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state identifier
        /// </summary>
        public int? StateID { get; set; }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        public string Zipcode { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the child.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the profile image url.
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the member type.
        /// </summary>
        public MemberType MemberType { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        public Gender? Gender { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the pay day auto approval
        /// </summary>
        public bool PayDayAutoApproval { get; set; }

        /// <summary>
        /// Gets or sets the is deleted
        /// </summary>
        /// <value>
        /// <c>true</c> if deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the is unsubscribed.
        /// </summary>
        /// <value>
        /// <c>true</c> if unsubscribed; otherwise, <c>false</c>.
        /// </value>
        public bool IsUnSubscribed { get; set; }

        /// <summary>
        /// Gets or sets the has trial.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user came from free trial link; otherwise, <c>false</c>.
        /// </value>
        public bool HasTrial { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public virtual State State { get; set; }

        /// <summary>
        /// Gets or sets user profile status.
        /// </summary>
        public ProfileStatus ProfileStatus { get; set; }

        /// <summary>
        /// Gets or sets user Promo code.
        /// </summary>
        public string PromoCode { get; set; }
    }
}
