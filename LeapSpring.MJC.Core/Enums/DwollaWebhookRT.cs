using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Enums
{
    /// <summary>
    /// Represents a dwolla webhooks response type
    /// </summary>
    public enum DwollaWebhookRT
    {
        [Description("transfer_created")]
        TransferCreated,

        [Description("transfer_completed")]
        TransferCompleted,

        [Description("transfer_cancelled")]
        TransferCancelled,

        [Description("transfer_failed")]
        TransferFailed,


        [Description("customer_created")]
        CustomerAdded,

        [Description("customer_verified")]
        CustomerVerified,

        [Description("customer_removed")]
        CustomerRemoved,

        [Description("customer_funding_source_added")]
        FundSourceAdded,

        [Description("customer_funding_source_removed")]
        FundSourceRemoved,

        [Description("customer_funding_source_verified")]
        FundSourceVerified,

        [Description("customer_funding_source_unverified")]
        FundSourceUnverified,

        [Description("customer_microdeposits_added")]
        MicroDepositsAdded,

        [Description("customer_microdeposits_completed")]
        MicroDepositsCompleted,

        [Description("customer_microdeposits_failed")]
        MicroDepositsFailed,

        [Description("bank_transfer_created")]
        BankTransferCreated,

        [Description("bank_transfer_completed")]
        BankTransferCompleted,

        [Description("bank_transfer_cancelled")]
        BankTransferCancelled,

        [Description("bank_transfer_failed")]
        BankTransferFailed,

        Wrong
    }
}
