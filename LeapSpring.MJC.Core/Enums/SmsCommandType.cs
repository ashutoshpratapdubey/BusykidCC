using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Enums
{
    /// <summary>
    /// Represents a sms command type
    /// </summary>
    public enum SmsCommandType
    {
        [Description("status")]
        KidStatus,

        [Description("balance")]
        KidBalance,

        [Description("my chores")]
        MyChoreStatus,

        [Description("yes")]
        Yes,

        [Description("no")]
        No,

        [Description("addchore")]
        AddChore,

        [Description("busykid joke")]
        Joke,

        [Description("bonus")]
        Bonus,

        [Description("busykid help")]
        Help,

        Wrong,

        [Description("always")]
        Always
    }
}
