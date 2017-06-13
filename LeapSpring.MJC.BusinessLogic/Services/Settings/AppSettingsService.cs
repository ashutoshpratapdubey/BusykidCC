using LeapSpring.MJC.Data.Repository;
using System.Collections.Specialized;
using System.Web.Configuration;
using System;

namespace LeapSpring.MJC.BusinessLogic.Services.Settings
{
    public class AppSettingsService : ServiceBase, IAppSettingsService
    {
        private NameValueCollection _appSettings;

        /// <summary>
        /// Gets or sets the Otp characters string.
        /// </summary>
        public string OtpCharacters { get; set; }

        /// <summary>
        /// Gets or sets the azure connection string.
        /// </summary>
        public string AzureConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the twilio account sid.
        /// </summary>
        public string TwilioAccountSid { get; set; }

        /// <summary>
        /// Gets or sets the twilio auth token.
        /// </summary>
        public string TwilioAuthToken { get; set; }

        /// <summary>
        /// Gets or sets the twilio phonw number.
        /// </summary>
        public string TwilioPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the child has phone receive bonus message
        /// </summary>
        public string ChildHasPhoneReceiveBonusMessage { get; set; }

        /// <summary>
        /// Gets or sets the child has no phone receive bonus message
        /// </summary>
        public string ChildHasNoPhoneReceiveBonusMessage { get; set; }

        /// <summary>
        /// Gets or sets the family invitation message
        /// </summary>
        public string FamilyInvitationMessage { get; set; }

        /// <summary>
        /// Gets or sets the child balance message
        /// </summary>
        public string ChildBalanceMessage { get; set; }

        /// <summary>
        /// Gets or sets the transaction completed child has phone message
        /// </summary>
        public string TransactionCompletedChildHasPhoneMessage { get; set; }

        /// <summary>
        /// Gets or sets the transaction completed child has not phone message
        /// </summary>
        public string TransactionCompletedChildHasNoPhoneMessage { get; set; }

        /// <summary>
        /// Gets or sets the site url
        /// </summary>
        public string SiteUrl { get; set; }

        /// <summary>
        /// Gets or set the is production
        /// </summary>
        public bool IsProduction { get; set; }

        /// <summary>
        /// Gets or sets the stock pile api baseurl.
        /// </summary>
        public string StockPileApiBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the stock pile api access identifier.
        /// </summary>
        public string StockPileAccessID { get; set; }

        /// <summary>
        /// Gets or sets the stock pile api secret key.
        /// </summary>
        public string StockPileAccessSecret { get; set; }

        /// <summary>
        /// Gets or sets the stock gift items api endpoint.
        /// </summary>
        public string StockGiftItemsEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the stock gift quotes api endpoint.
        /// </summary>
        public string StockGiftQuotesEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the stock pile purchase api endpoint.
        /// </summary>
        public string StockPilePurchaseEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the stock pile order api endpoint.
        /// </summary>
        public string OrderStockEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the stock pile order cancellation api endpoint.
        /// </summary>
        public string CancelStockOrderEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the corepro domain name.
        /// </summary>
        public string CoreProDomainName { get; set; }

        /// <summary>
        /// Gets or sets the corepro api key.
        /// </summary>
        public string CoreProApiKey { get; set; }

        /// <summary>
        /// Gets or sets the corepro api secret.
        /// </summary>
        public string CoreProApiSecret { get; set; }

        /// <summary>
        /// Gets or sets the culture for corepro api.
        /// </summary>
        public string CoreProCulture { get; set; }

        /// <summary>
        /// Gets or sets the plaid api base url.
        /// </summary>
        public string PlaidBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the plaid api client identifier.
        /// </summary>
        public string PlaidClientID { get; set; }

        /// <summary>
        /// Gets or sets the plaid api client secret.
        /// </summary>
        public string PlaidClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the azure service bus connection string.
        /// </summary>
        public string AzureServiceBusConnectionString { get; set; }

        public string AzureServiceBusConnectionStringTest { get; set; }

        //CorePro Api Transaction path details

        public string CoreproTransactionDetailFilePath { get; set; }
        public string coreProSftpHost { get; set; }
        public string coreProSftpPort { get; set; }
        public string coreProSftpRemoteFolder { get; set; }
        public string coreProSftpUserName { get; set; }
        public string coreProSftpPassword { get; set; }

        public AppSettingsService(IRepository repository) : base(repository)
        {
            _appSettings = WebConfigurationManager.AppSettings;
            GetSettings();
        }

        /// <summary>
        /// Gets the settings from web.config
        /// </summary>
        public void GetSettings()
        {
            OtpCharacters = _appSettings.Get("otpCharacters");
            AzureConnectionString = _appSettings.Get("azureConnectionString");

            TwilioAccountSid = _appSettings.Get("twilioAccountSid");
            TwilioAuthToken = _appSettings.Get("twilioAuthToken");
            TwilioPhoneNumber = _appSettings.Get("twilioPhoneNumber");
            ChildHasPhoneReceiveBonusMessage = _appSettings.Get("childHasPhoneReceiveBonusMessage");
            ChildHasNoPhoneReceiveBonusMessage = _appSettings.Get("childHasNoPhoneReceiveBonusMessage");
            FamilyInvitationMessage = _appSettings.Get("familyInvitationMessage");
            ChildBalanceMessage = _appSettings.Get("childBalanceMessage");
            TransactionCompletedChildHasPhoneMessage = _appSettings.Get("transactionCompletedChildHasPhoneMessage");
            TransactionCompletedChildHasNoPhoneMessage = _appSettings.Get("transactionCompletedChildHasNoPhoneMessage");
            SiteUrl = _appSettings.Get("siteUrl");
            IsProduction = bool.Parse(_appSettings.Get("isProduction"));

            StockPileApiBaseUrl = _appSettings.Get("StockPileBaseUrl");
            StockPileAccessID = _appSettings.Get("SPAccessID");
            StockPileAccessSecret = _appSettings.Get("SPAccessSecret");

            StockGiftItemsEndPoint = _appSettings.Get("stockGiftItems");
            StockGiftQuotesEndPoint = _appSettings.Get("stockGiftQuotes");
            StockPilePurchaseEndPoint = _appSettings.Get("purchaseStock");
            OrderStockEndPoint = _appSettings.Get("orderStock");
            CancelStockOrderEndPoint = _appSettings.Get("cancelStock");

            CoreProDomainName = _appSettings.Get("CoreProDomainName");
            CoreProApiKey = _appSettings.Get("CoreProApiKey");
            CoreProApiSecret = _appSettings.Get("CoreProApiSecret");
            CoreProCulture = _appSettings.Get("CoreProCulture");

            PlaidBaseUrl = _appSettings.Get("PlaidBaseUrl");
            PlaidClientID = _appSettings.Get("PlaidClientID");
            PlaidClientSecret = _appSettings.Get("PlaidClientSecret");
            AzureServiceBusConnectionString= _appSettings.Get("azureServiceBusConnectionString");
            AzureServiceBusConnectionStringTest = _appSettings.Get("azureServiceBusConnectionStringTest");

            //CorePro Api Transaction path details

            CoreproTransactionDetailFilePath = _appSettings.Get("CoreproTransactionDetailFilePath");
            coreProSftpHost = _appSettings.Get("sftpHost");
            coreProSftpPort = _appSettings.Get("sftpPort");
            coreProSftpRemoteFolder = _appSettings.Get("sftpRemoteFolder");
            coreProSftpUserName = _appSettings.Get("sftpUserName");
            coreProSftpPassword = _appSettings.Get("sftpPassword");
        }
    }
}
