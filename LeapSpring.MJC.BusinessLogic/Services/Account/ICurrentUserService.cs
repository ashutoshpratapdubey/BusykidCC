using LeapSpring.MJC.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Account
{
    public interface ICurrentUserService
    {
        #region User Context

        /// <summary>
        /// Gets the family identifier
        /// </summary>
        int FamilyID { get; }

        /// <summary>
        /// Gets the member identifier
        /// </summary>
        int MemberID { get; }

        /// <summary>
        /// Gets the member type
        /// </summary>
        MemberType MemberType { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is logged in.
        /// </summary>
        bool IsLoggedIn { get; }

        #endregion
    }
}
