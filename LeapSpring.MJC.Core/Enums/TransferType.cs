using System.ComponentModel;

namespace LeapSpring.MJC.Core.Enums
{
    public enum TransferType
    {
        [Description("Deposit from external account")]
        ExternalToInetrnalAccount,

        [Description("Withdrawal to external account")]
        InternalToExternalAccount,

        [Description("Deposit from client internal account to busykid internal account")]
        InternalToBusyKidInternalAccount
    }
}
