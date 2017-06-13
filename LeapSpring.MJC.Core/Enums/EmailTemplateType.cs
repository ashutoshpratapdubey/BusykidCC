using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Enums
{
    /// <summary>
    /// Represents a email template type
    /// </summary>
    public enum EmailTemplateType
    {
        BankTransferCreated,
        BankTransferCompleted,
        BankTransferFailed,
        BankTransferCancelled,

        DwollaCustomerCreated,
        DwollaFundSourceAdded,
        DwollaFundSourceVerified,
        DwollaFundSourceRemoved,
        IAVWelcome,
        MicroDepositWelcome,
        MicroDepositAdded,
        MicroDepositCompleted,
        MicroDepositFailed,

        IncompleteNewMemberEnrollment,
        PasswordReset,
        NoChildrenEnrolled,
        EmailAddressChange,
        ParentHasNotLoggedInThreeDays,
        ParentHasNotLoggedInTwoWeeks,
        ChildHasNotLoggedInThreeDays,
        ChildHasNotLoggedInSevenDays,
        ChildHasNotLoggedInTwoWeeks,
        NoChoreCompletedThreeDays,
        NoChoreCompletedSevenDays,
        SevenStraightDaysofActivity,
        TwoWeeksStraightDaysofActivity,
        OneMonthStraightDaysofActivity,
        OneYearRenewalApproaching,
        OneYearRenewal,
        AccountCancellation,
        NotificationAccountVerify,
        Pinupdated,
        PasswordUpdate
    }
}
