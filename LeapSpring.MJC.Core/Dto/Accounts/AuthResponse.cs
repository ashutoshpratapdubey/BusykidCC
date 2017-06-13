using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System.Collections.Generic;

namespace LeapSpring.MJC.Core.Dto.Accounts
{
    public class AuthResponse
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
        /// Gets or sets the profile image url.
        /// </summary>
        public string ProfileUrl { get; set; }

        /// <summary>
        /// Gets or sets the member type.
        /// </summary>
        public MemberType MemberType { get; set; }

        /// <summary>
        /// Gets or sets the family name.
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// Gets or sets the family identifier.
        /// </summary>
        public int FamilyId { get; set; }

        /// <summary>
        /// Gets or sets the family member identifier.
        /// </summary>
        public int FamilyMemberId { get; set; }

        /// <summary>
        /// Gets or sets the family url
        /// </summary>
        public string FamilyUrl { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
