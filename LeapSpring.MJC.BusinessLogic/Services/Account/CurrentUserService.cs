using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LeapSpring.MJC.BusinessLogic.Services.Account
{
    public class CurrentUserService : ICurrentUserService
    {
        #region User Context

        /// <summary>
        /// Gets the family identifier
        /// </summary>
        public int FamilyID
        {
            get
            {
                var user = HttpContext.Current?.User as ClaimsPrincipal;
                if (user == null)
                    throw new UnauthorizedAccessException();

                return int.Parse(user.Claims.SingleOrDefault(u => u.Type == "FamilyID").Value);
            }
        }


        /// <summary>
        /// Gets the member identifier
        /// </summary>
        public int MemberID
        {
            get
            {
                var user = HttpContext.Current?.User as ClaimsPrincipal;
                if (user == null)
                    throw new UnauthorizedAccessException();

                return int.Parse(user.Claims.SingleOrDefault(u => u.Type == "MemberID").Value);
            }
        }

        /// <summary>
        /// Gets the member type
        /// </summary>
        public MemberType MemberType
        {
            get
            {
                var user = HttpContext.Current?.User as ClaimsPrincipal;
                if (user == null)
                    throw new UnauthorizedAccessException();

                return (MemberType)int.Parse(user.Claims.SingleOrDefault(u => u.Type == "MemberType").Value);
            }
        }


        /// <summary>
        /// Gets a value indicating whether this instance is logged in.
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                var user = HttpContext.Current?.User as ClaimsPrincipal;
                return user.Identities.ToList()[0].IsAuthenticated;
            }
        }

        #endregion
    }
}
