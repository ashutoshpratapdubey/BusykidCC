using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Enums
{
    /// <summary>
    /// Represents a dwolla customer status
    /// </summary>
    public enum DwollaAccountStatus
    {
        NotLinked,
        Added,
        MicroDepositAdded,
        MicroDepositCompleted,
        Verified,
        Unverified
    }
}
