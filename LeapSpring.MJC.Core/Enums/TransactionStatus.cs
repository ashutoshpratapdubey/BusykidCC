using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Enums
{
    /// <summary>
    /// Represents a bank transfer status
    /// </summary>
    public enum TransactionStatus
    {
        [Description("pending")]
        Pending,

        [Description("completed")]
        Completed,

        [Description("failed")]
        Failed,

        [Description("cancelled")]
        Cancelled,

        [Description("reclaimed")]
        Reclaimed,

        [Description("settled")]
        Settled
    }
}
