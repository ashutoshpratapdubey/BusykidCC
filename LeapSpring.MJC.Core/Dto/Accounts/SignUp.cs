using LeapSpring.MJC.Core.Enums;

namespace LeapSpring.MJC.Core.Dto.Accounts
{
    public class SignUp
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
        /// Gets or sets the zipcode.
        /// </summary>
        public string Zipcode { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the master password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the member type.
        /// </summary>
        public MemberType  MemberType { get; set; }

        /// <summary>
        /// Gets or sets the family identifier
        /// </summary>
        public int FamilyID { get; set; }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the has trial.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user came from free trial link; otherwise, <c>false</c>.
        /// </value>
        public bool HasTrial { get; set; }

        /// <summary>
        /// Gets or sets the has Promo code.
        /// </summary>
         public string PromoCode { get; set; }
    }
}
