using CorePro.SDK;
using CorePro.SDK.Models;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.Core.Domain.Family;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    public class CoreProService : ICoreProService
    {
        private IAppSettingsService _appSettingsService;
        private Connection _connection;

        #region Ctor

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="appSettingsService">The app settings service.</param>
        public CoreProService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
            _connection = GetConnection();
        }

        #endregion

        #region public Methods

        /// <summary>
        /// Creates the corepro customer.
        /// </summary>
        /// <param name="adminMember">The admin member of the family</param>
        /// <returns>The corepro customer.</returns>
        public Customer CreateCustomer(FamilyMember adminMember)
        {
            _connection = _connection ?? GetConnection();
            var customer = Customer.Create(_appSettingsService.CoreProCulture, false, false, true, string.Empty,
                string.Empty, adminMember.Firstname, string.Empty, adminMember.Lastname, string.Empty, null,
                adminMember.User.Email, string.Empty, string.Empty, null, string.Empty, string.Empty, null, null, _connection);

            return customer;
        }

        /// <summary>
        /// Creates the corepro account for the customer.
        /// </summary>
        /// <param name="customerId">The corepro customer identifier.</param>
        /// <param name="accountName">The name of the account.</param>
        /// <returns>The corepro account identifier.</returns>
        public int CreateAccount(int customerId, string accountName)
        {
            _connection = _connection ?? GetConnection();

            return CorePro.SDK.Account.Create(customerId, null,
                accountName, "ForBenefitOf", string.Empty, string.Empty, string.Empty, null, null, null,
                string.Empty, null, null, null, null, true, _connection);
        }

        /// <summary>
        ///  Creates the external bank account for the customer.
        /// </summary>
        /// <param name="customerID">The corepro customer identifier.</param>
        /// <param name="institutionName">The bank name.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="accountType">The external account type.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="isTrialDeposit">The is trial deposit.</param>
        /// <returns>The external account</returns>
        public ExternalAccount CreateExternalAccount(int customerID, string institutionName, string firstName, string lastName, string accountType, string routingNumber, string accountNumber, bool isTrialDeposit = false)
        {
            try
            {
                _connection = _connection ?? GetConnection();

                var externalAccountId = 0;
                if (!isTrialDeposit)
                {
                    //Creates the external account
                    externalAccountId = ExternalAccount.Create(customerID, institutionName, firstName, lastName, accountType,
                    routingNumber, accountNumber, string.Empty, string.Empty, _connection);
                }
                else
                {
                    //Creates the external account with micro deposit
                    externalAccountId = ExternalAccount.Initiate(customerID, institutionName, firstName, lastName, accountType,
                        routingNumber, accountNumber, string.Empty, string.Empty, _connection);
                }

                //Gets the external account by external account identifier
                return GetExternalAccount(customerID, externalAccountId);
            }
            catch (CoreProApiException ex)
            {
                var message = ex?.Errors?.FirstOrDefault()?.Message;
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Verifies an external account.
        /// </summary>
        /// <param name="customerID">The corepro customer identifier.</param>
        /// <param name="externalAccountId">The external account identifier.</param>
        /// <param name="firstAmount">The first amount.</param>
        /// <param name="secondAmount">The second amount.</param>
        /// <returns><c>True</c>, If verified. Otherwise, <c>False</c>.</returns>
        public bool VerifyExternalAccount(int customerID, int externalAccountId, decimal firstAmount, decimal secondAmount)
        {
            try
            {
                _connection = _connection ?? GetConnection();

                return ExternalAccount.Verify(customerID, externalAccountId, firstAmount, secondAmount, _connection);
            }
            catch (CoreProApiException ex)
            {
                var message = ex?.Errors?.FirstOrDefault()?.Message;
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Creates the corepro money transfer.
        /// </summary>
        /// <param name="customerId">The corepro customer identifier.</param>
        /// <param name="fromAccountId">The source account identifier.</param>
        /// <param name="toAccountId">The destination account identifier.</param>
        /// <param name="amount">The amount to be transferred.</param>
        /// <returns>The list of transfers created.</returns>
        public IList<Transfer> CreateTransfer(int customerId, int fromAccountId, int toAccountId, decimal amount)
        {
            try
            {
                _connection = _connection ?? GetConnection();

                //Creates the corepro transfer.
                return Transfer.Create(customerId, fromAccountId, toAccountId, amount, string.Empty, string.Empty, _connection);
            }
            catch (CoreProApiException ex)
            {
                var message = ex?.Errors?.FirstOrDefault()?.Message;
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Gets the corepro account.
        /// </summary>
        /// <param name="customerId">The corepro customer identifier.</param>
        /// <param name="accountId">The corepro account identifier.</param>
        /// <returns>The corepro account</returns>
        public CorePro.SDK.Account GetAccount(int customerId, int accountId)
        {
            _connection = _connection ?? GetConnection();
            return CorePro.SDK.Account.Get(customerId, accountId, _connection);
        }

        /// <summary>
        /// Gets the corepro account.
        /// </summary>
        /// <param name="customerId">The corepro customer identifier.</param>
        /// <param name="externalAccountId">The external account identifier.</param>
        /// <returns>The corepro account</returns>
        public ExternalAccount GetExternalAccount(int customerId, int externalAccountId)
        {
            _connection = _connection ?? GetConnection();
            return ExternalAccount.Get(customerId, externalAccountId, _connection);
        }

        /// <summary>
        /// Gets the bank documents
        /// </summary>
        /// <returns>The list of the bank documents.</returns>
        public List<Document> GetBankDocuments()
        {
            _connection = _connection ?? GetConnection();
            return Document.List(_appSettingsService.CoreProCulture, null, _connection);
        }

        /// <summary>
        /// Gets the bank documents by document identifier.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>The file content.</returns>
        public FileContent GetBankDocumentById(int documentId)
        {
            _connection = _connection ?? GetConnection();
            return BankDocument.Download(_appSettingsService.CoreProCulture, documentId, _connection);
        }

        /// <summary>
        /// Disconnects the external account for corepro
        /// </summary>
        /// <param name="customerId">The corepro customer identifier.</param>
        /// <param name="externalAccountId">The external account identifier.</param>
        /// <returns></returns>
        public bool ArchiveExternalAccount(int customerId, int externalAccountId)
        {
            try
            {
                _connection = _connection ?? GetConnection();
                return ExternalAccount.Archive(customerId, externalAccountId, _connection);
            }
            catch (CoreProApiException ex)
            {
                var message = ex?.Errors?.FirstOrDefault()?.Message;
                throw new InvalidOperationException(message);
            }
        }

        #endregion

        /// <summary>
        /// Gets the corepro api connection
        /// </summary>
        /// <returns></returns>
        private Connection GetConnection()
        {
            return new Connection(_appSettingsService.CoreProDomainName, _appSettingsService.CoreProApiKey, _appSettingsService.CoreProApiSecret);
        }


        public string UpdateEmail(int cutomerid, string email)
        {
            _connection = _connection ?? GetConnection();

            var customer = Customer.Update(cutomerid, null, null, null, null, null, null, null, null, null, null, null, null, null, null, email, null, null,
               true, null, null, null, null, null, null, null, null, null, _connection, null);

            return customer.ToString();
        }

        public Customer get(int cutomerid)
        {
            _connection = _connection ?? GetConnection();
            var customer = Customer.Get(cutomerid);

            return customer;
        }
    }
}
