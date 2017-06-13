using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Enums;

namespace LeapSpring.MJC.BusinessLogic.Services.Settings
{
    public interface IAppSettingsService
    {
        /// <summary>
        /// Gets or sets the Otp characters string.
        /// </summary>
        string OtpCharacters { get; set; }

        /// <summary>
        /// Gets or sets the azure connection string.
        /// </summary>
        string AzureConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the twilio account sid.
        /// </summary>
        string TwilioAccountSid { get; set; }

        /// <summary>
        /// Gets or sets the twilio auth token.
        /// </summary>
        string TwilioAuthToken { get; set; }

        /// <summary>
        /// Gets or sets the twilio phonw number.
        /// </summary>
        string TwilioPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the child has phone receive bonus message
        /// </summary>
        string ChildHasPhoneReceiveBonusMessage { get; set; }

        /// <summary>
        /// Gets or sets the child has not phone receive bonus message
        /// </summary>
        string ChildHasNoPhoneReceiveBonusMessage { get; set; }

        /// <summary>
        /// Gets or sets the family invitation message
        /// </summary>
        string FamilyInvitationMessage { get; set; }

        /// <summary>
        /// Gets or sets the child balance message
        /// </summary>
        string ChildBalanceMessage { get; set; }

        /// <summary>
        /// Gets or sets the transaction completed child has phone message
        /// </summary>
        string TransactionCompletedChildHasPhoneMessage { get; set; }

        /// <summary>
        /// Gets or sets the transaction completed child has not phone message
        /// </summary>
        string TransactionCompletedChildHasNoPhoneMessage { get; set; }

        /// <summary>
        /// Gets or sets the site url
        /// </summary>
        string SiteUrl { get; set; }

        /// <summary>
        /// Gets or set the is production
        /// </summary>
        bool IsProduction { get; set; }

        /// <summary>
        /// Gets or sets the stock pile api baseurl.
        /// </summary>
        string StockPileApiBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the stock pile api access identifier.
        /// </summary>
        string StockPileAccessID { get; set; }

        /// <summary>
        /// Gets or sets the stock pile api secret key.
        /// </summary>
        string StockPileAccessSecret { get; set; }

        /// <summary>
        /// Gets or sets the stock gift items api endpoint.
        /// </summary>
        string StockGiftItemsEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the stock gift quotes api endpoint.
        /// </summary>
        string StockGiftQuotesEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the stock pile purchase api endpoint.
        /// </summary>
        string StockPilePurchaseEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the stock pile order api endpoint.
        /// </summary>
        string OrderStockEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the stock pile order cancellation api endpoint.
        /// </summary>
        string CancelStockOrderEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the corepro domain name.
        /// </summary>
        string CoreProDomainName { get; set; }

        /// <summary>
        /// Gets or sets the corepro api key.
        /// </summary>
        string CoreProApiKey { get; set; }

        /// <summary>
        /// Gets or sets the corepro api secret.
        /// </summary>
        string CoreProApiSecret { get; set; }

        /// <summary>
        /// Gets or sets the culture for corepro api.
        /// </summary>
        string CoreProCulture { get; set; }

        /// <summary>
        /// Gets or sets the plaid api base url.
        /// </summary>
        string PlaidBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the plaid api client identifier.
        /// </summary>
        string PlaidClientID { get; set; }

        /// <summary>
        /// Gets or sets the plaid api client secret.
        /// </summary>
        string PlaidClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the azure service bus connection string.
        /// </summary>
        string AzureServiceBusConnectionString { get; set; }


        /// <summary>
        /// Gets or sets the azure service bus connection string.
        /// </summary
        string AzureServiceBusConnectionStringTest { get; set; }

        //CorePro Api Transaction path details
        string CoreproTransactionDetailFilePath { get; set; }
        string coreProSftpHost { get; set; }
        string coreProSftpPort { get; set; }
        string coreProSftpRemoteFolder { get; set; }
        string coreProSftpUserName { get; set; }
        string coreProSftpPassword { get; set; }

        /// <summary>
        /// Gets the settings from web.config
        /// </summary>
        void GetSettings();
    }
}
