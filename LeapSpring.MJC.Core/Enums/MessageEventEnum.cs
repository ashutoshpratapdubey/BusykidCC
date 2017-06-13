namespace LeapSpring.MJC.Core.Enums
{
    public enum MessageEventEnum
    {
        LockedExternalAccount = 3008,

        UnlockedExternalAccount = 3009,

        CreatedExternalAccount = 3010,

        ModifiedExternalAccount = 3011,

        VerifiedExternalAccount = 3012,

        ResendVerifyExternalAccount = 3013,

        VoidedAccountTransaction = 3014,

        VoidedExternalAccountTransaction = 3015,

        ReversedAccountTransaction = 3016,

        ReversedExternalAccountTransaction = 3017,

        AddedFee_or_Credit_to_CustomerAccount = 3018,

        FundingSourceVerified = 3058,

        CreatedCustomerAccount = 3059,

        CreatedAccountTransaction = 3060,

        TransactionSettled = 3101,

        TransactionAvailable = 3102,
    }
}
