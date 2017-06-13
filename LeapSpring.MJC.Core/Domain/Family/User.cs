using System;

namespace LeapSpring.MJC.Core.Domain.Family
{
    public class User : BaseEntity
    {
        /// <summary>
        /// Gets ot sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the master password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password salt.
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets the PIN.
        /// </summary>
        public string PIN { get; set; }

        /// <summary>
        /// Gets ot sets the family identifier.
        /// </summary>
        public int FamilyID { get; set; }

        /// <summary>
        /// Gets or sets the family.
        /// </summary>
        public virtual Family Family { get; set; }

        /// <summary>
        /// Gets or sets the logged on.
        /// </summary>
        public DateTime LoggedOn { get; set; }
    }
}
