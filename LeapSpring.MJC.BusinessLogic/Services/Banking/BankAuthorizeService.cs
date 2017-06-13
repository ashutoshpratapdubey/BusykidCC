using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Threading.Tasks;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using CorePro.SDK;
using System.Collections.Generic;
using CorePro.SDK.Models;
using LeapSpring.MJC.BusinessLogic.Services.Emails;
using System.Net;
using System.IO;
using System.Xml;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    /// <summary>
    /// Represents a interface bank authorize service
    /// </summary>
    public class BankAuthorizeService : ServiceBase, IBankAuthorizeService
    {
        ICurrentUserService _currentUserService;
        private ISignUpProgressService _signUpProgressService;
        private IAppSettingsService _appSettingsService;
        private ICoreProService _coreproService;
        private IFamilyService _familyService;
        private IBankService _bankService;
        private IPlaidService _plaidService;
        private IEmailService _emailService;
        private IEmailTemplateService _emailTemplateService;
        private IEmailHistoryService _emailHistoryService;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Repository</param>
        public BankAuthorizeService(IRepository repository, ICurrentUserService currentUserService, ISignUpProgressService signUpProgressService,
            IAppSettingsService appSettingsService, ICoreProService coreproService, IFamilyService familyService, IBankService bankService,
            IPlaidService plaidService, IEmailService emailService, IEmailTemplateService emailTemplateService, IEmailHistoryService emailHistoryService) : base(repository)
        {
            _currentUserService = currentUserService;
            _signUpProgressService = signUpProgressService;
            _appSettingsService = appSettingsService;
            _coreproService = coreproService;
            _familyService = familyService;
            _bankService = bankService;
            _plaidService = plaidService;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _emailHistoryService = emailHistoryService;
        }

        #region CorePro

        /// <summary>
        /// Creates the corepro customer and account.
        /// </summary>
        public void CreateCustomer()
        {
            if (_bankService.HasFinancialAccount())
                return;

            var familyMember = _familyService.GetMember();

            // Creates the corepro customer
            var customer = _coreproService.CreateCustomer(familyMember);

            // Creates the corepro account
            var accountId = _coreproService.CreateAccount(customer.CustomerId.Value, familyMember.Firstname);

            // Creates the new finance account
            var financialAccount = new FinancialAccount
            {
                CustomerID = customer.CustomerId.Value,
                AccountID = accountId,
                FamilyMemberID = _currentUserService.MemberID,
            };
            Repository.Insert(financialAccount);
        }

        /// <summary>
        /// Links the bank account.
        /// </summary>
        /// <param name="publicToken">The public key token.</param>
        /// <param name="institutionName">Name of the institution.</param>
        /// <param name="selectedAccountId">Selected account identifier</param>
        public async Task LinkBankAccount(string publicToken, string institutionName, string selectedAccountId)
        {
            var accessToken = await _plaidService.LifeTimeAccessToken(publicToken);
            if (string.IsNullOrEmpty(accessToken))
                throw new InvalidParameterException("Invalid credentials!");

            var bankInfo = await _plaidService.GetBankInfo(accessToken, selectedAccountId);
            if (bankInfo == null)
                throw new InvalidParameterException("Unable to find your bank!");

            var financialAccount = _bankService.GetFinancialAccount();
            if (financialAccount != null && financialAccount.ExternalAccountID.HasValue)
                await _bankService.RemoveBank();

            // Creates the external account
            var externalAccount = _coreproService.CreateExternalAccount(financialAccount.CustomerID, institutionName, financialAccount.FamilyMember.Firstname, financialAccount.FamilyMember.Lastname, bankInfo.AccountSubType, bankInfo.RoutingNumber, bankInfo.AccountNumber);

            // Updates the external account identifier.
            financialAccount.ExternalAccountID = externalAccount.ExternalAccountId;
            financialAccount.BankName = externalAccount.Name;
            financialAccount.PlaidAccessToken = accessToken;
            financialAccount.MaskedAccountNumber = externalAccount.AccountNumberMasked;
            financialAccount.Status = (FinancialAccountStatus)Enum.Parse(typeof(FinancialAccountStatus), externalAccount.Status.ToString());
            financialAccount.AccountType = FundingSourceType.InstantAccount;
            Repository.Update(financialAccount);

            if (financialAccount.Status == FinancialAccountStatus.Verified)
            {
                // Updates the signup progress.
                if (financialAccount.FamilyMember.User.Family.SignUpStatus != SignUpStatus.Completed)
                    _signUpProgressService.UpdateSignUpProgress(SignUpStatus.Completed, financialAccount.FamilyMember.User.FamilyID);

                var hasSentWelcomeEmail = _emailHistoryService.HasSent(financialAccount.FamilyMemberID, EmailType.NewMemberEnrollmentNonMJC);
                if (!hasSentWelcomeEmail && !financialAccount.FamilyMember.IsUnSubscribed)
                {
                    var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.IAVWelcome);
                    _emailHistoryService.SaveEmailHistory(financialAccount.FamilyMemberID, EmailType.NewMemberEnrollmentNonMJC);

                    await _emailService.Send(financialAccount.FamilyMember.User.Email, emailTemplate.Subject, emailTemplate.Content);
                }
            }
        }

        /// <summary>
        /// Links the micro deposit account.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="routingNumber">The routing number.</param>
        /// <param name="accountType">The account type.</param>
        public async Task LinkMicroDepositAccount(string accountNumber, string routingNumber, string accountType)
        {
           
            CreateCustomer();

            var financialAccount = _bankService.GetFinancialAccount();
            if (financialAccount != null && financialAccount.ExternalAccountID.HasValue)
                await _bankService.RemoveBank();

            // Creates the external account
            var externalAccount = _coreproService.CreateExternalAccount(financialAccount.CustomerID, string.Empty, financialAccount.FamilyMember.Firstname, financialAccount.FamilyMember.Lastname, accountType, routingNumber, accountNumber, true);

            // Updates the external account identifier.
            financialAccount.ExternalAccountID = externalAccount.ExternalAccountId;
            financialAccount.BankName = externalAccount.Name;
            financialAccount.MaskedAccountNumber = externalAccount.AccountNumberMasked;
            financialAccount.Status = (FinancialAccountStatus)Enum.Parse(typeof(FinancialAccountStatus), externalAccount.Status.ToString());
            financialAccount.AccountType = FundingSourceType.MicroDeposit;
            Repository.Update(financialAccount);

            // Updates the signup progress.
            _signUpProgressService.UpdateSignUpProgress(financialAccount.FamilyMember.User.FamilyID);

          

        }
       

        /// <summary>
        /// Verifies the bank account.
        /// </summary>
        /// <param name="firstAmount">The first amount received.</param>
        /// <param name="secondAmount">The second amoount received.</param>
        public async Task VerifyBankAccount(decimal firstAmount, decimal secondAmount)
        {
            var financialAccount = _bankService.GetFinancialAccount(_currentUserService.MemberID);
            var isVerified = _coreproService.VerifyExternalAccount(financialAccount.CustomerID, financialAccount.ExternalAccountID.Value, firstAmount, secondAmount);
            var verificationStatus = FinancialAccountStatus.Verified;

            if (isVerified)
            {
                // Updates the signup progress.
                if (financialAccount.FamilyMember.User.Family.SignUpStatus != SignUpStatus.Completed)
                    _signUpProgressService.UpdateSignUpProgress(SignUpStatus.Completed, financialAccount.FamilyMember.User.FamilyID);

                var hasSentWelcomeEmail = _emailHistoryService.HasSent(financialAccount.FamilyMemberID, EmailType.NewMemberEnrollmentNonMJC);
                if (!hasSentWelcomeEmail && !financialAccount.FamilyMember.IsUnSubscribed)
                {
                    var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.MicroDepositWelcome);
                    _emailHistoryService.SaveEmailHistory(financialAccount.FamilyMemberID, EmailType.NewMemberEnrollmentNonMJC);

                    await _emailService.Send(financialAccount.FamilyMember.User.Email, emailTemplate.Subject, emailTemplate.Content);
                }
            }
            else
            {
                var externalAccount = _coreproService.GetExternalAccount(financialAccount.CustomerID, financialAccount.ExternalAccountID.Value);
                verificationStatus = (FinancialAccountStatus)Enum.Parse(typeof(FinancialAccountStatus), externalAccount.Status.ToString());
            }
            financialAccount.Status = verificationStatus;
            Repository.Update(financialAccount);
        }

        /// <summary>
        /// Gets the bank documents.
        /// </summary>
        /// <returns>The list of bank documents to be displayed.</returns>
        public List<Document> GetBankDocuments()
        {
            return _coreproService.GetBankDocuments();
        }

        /// <summary>
        /// Gets the bank documents by document identifier.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>The file content.</returns>
        public FileContent GetBankDocumentById(int documentId)
        {
            return _coreproService.GetBankDocumentById(documentId);
        }
        
        #endregion
    }
}
