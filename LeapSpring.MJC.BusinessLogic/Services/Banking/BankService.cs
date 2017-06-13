using System.Linq;
using LeapSpring.MJC.Data.Repository;
using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.Core.Enums;
using System.Data.Entity;
using System;
using System.Threading.Tasks;
using LeapSpring.MJC.Core.Domain.Family;
using System.Xml;
using System.Net;
using System.IO;
using System.Configuration;

namespace LeapSpring.MJC.BusinessLogic.Services.Banking
{
    public class BankService : ServiceBase, IBankService
    {
        private ICurrentUserService _currentUserService;
        private IPlaidService _plaidService;
        private ICoreProService _coreproService;
        private ISignUpProgressService _signUpProgressService;

        public BankService(IRepository repository, ICurrentUserService currentUserService, IPlaidService plaidService, ICoreProService coreproService, ISignUpProgressService signUpProgressService) : base(repository)
        {
            _currentUserService = currentUserService;
            _plaidService = plaidService;
            _coreproService = coreproService;
            _signUpProgressService = signUpProgressService;
        }

        /// <summary>
        /// Is bank account linked
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <returns>Result [IsLinked]</returns>
        public bool IsBankLinked(int? familyMemberId = null)
        {
            var memberId = familyMemberId ?? _currentUserService.MemberID;
            return Repository.Table<FinancialAccount>().Any(m => m.FamilyMemberID == memberId && m.ExternalAccountID.HasValue && m.Status == FinancialAccountStatus.Verified);
        }

        /// <summary>
        /// Gets the financial account.
        /// </summary>
        /// <param name="adminMemberId">The admin member identifier of the family.</param>
        /// <returns>The financial account.</returns>
        public FinancialAccount GetFinancialAccount(int? adminMemberId = null)
        {
            var memberId = adminMemberId ?? _currentUserService.MemberID;
            return Repository.Table<FinancialAccount>().Include(p => p.FamilyMember).Include(p => p.FamilyMember.User)
                .Include(p => p.FamilyMember.User.Family).SingleOrDefault(p => p.FamilyMemberID == memberId);
        }

        /// <summary>
        /// checks the member has financial account or not. 
        /// </summary>
        /// <param name="adminMemberId">The admin member identifier of the family.</param>
        /// <returns><c>True</c> if not finance account present. otherwise, <c>False</c>.</returns>
        public bool HasFinancialAccount(int? adminMemberId = null)
        {
            var memberId = adminMemberId ?? _currentUserService.MemberID;
            return Repository.Table<FinancialAccount>().Any(p => p.FamilyMemberID == memberId);
        }

        /// <summary>
        /// Removes the bank account
        /// </summary>
        public async Task RemoveBank()
        {
            var financialAccount = GetFinancialAccount();
            if (financialAccount == null)
                return;

            if (financialAccount.AccountType == FundingSourceType.InstantAccount && !string.IsNullOrEmpty(financialAccount.PlaidAccessToken))
            {
                // Removes the plaid user
                var isRemoved = await _plaidService.RemoveBankInfo(financialAccount.PlaidAccessToken);
                if (!isRemoved)
                    throw new InvalidOperationException("Falied to remove your bank account!");
            }

            // Removes the external bank account.
            var isArchived = _coreproService.ArchiveExternalAccount(financialAccount.CustomerID, financialAccount.ExternalAccountID.Value);
            if (!isArchived)
                throw new InvalidOperationException("Falied to remove your bank account!");

            // Removes the bank linked.
            financialAccount.ExternalAccountID = null;
            financialAccount.BankName = string.Empty;
            financialAccount.PlaidAccessToken = string.Empty;
            financialAccount.MaskedAccountNumber = string.Empty;
            financialAccount.Status = FinancialAccountStatus.NotLinked;
            financialAccount.AccountType = null;
            Repository.Update(financialAccount);

            // Updates the signup progress.
            _signUpProgressService.UpdateSignUpProgress(financialAccount.FamilyMember.User.FamilyID);
        }


        public CreditCardAccount GetCreditCard()
        {
            return Repository.Table<CreditCardAccount>().SingleOrDefault(p => p.FamilyMemberID == _currentUserService.MemberID);
        }



        public CreditCardAccount GetCreditCardInfo()
        {

            var familymember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id == _currentUserService.MemberID);

            var familyID = Repository.Table<User>().Where(m => m.Id == familymember.Id).FirstOrDefault().FamilyID;
            var AdminFamilyMemberID = Repository.Table<User>().Where(m => m.FamilyID == familyID && m.Email != null).FirstOrDefault().Id;

            return Repository.Table<CreditCardAccount>().SingleOrDefault(p => p.FamilyMemberID == AdminFamilyMemberID);
        }

        public CreditCardAccount UpdateCardStatus()
        {
            var CardInfo = Repository.Table<CreditCardAccount>().SingleOrDefault(p => p.FamilyMemberID == _currentUserService.MemberID);
            if (CardInfo != null)
            {
                CardInfo.CardStatus = 0;
                Repository.Update(CardInfo);
            }

            return CardInfo;
        }


        #region CreditCard
        public string sendXMLRequest(XmlDocument xmlRequest)
        {

            string uri = ConfigurationManager.AppSettings["CardflexUrl"];
            WebRequest req = WebRequest.Create(uri);           
            req.Method = "POST";        // Post method
            req.ContentType = "text/xml";     // content type           
            StreamWriter writer = new StreamWriter(req.GetRequestStream());
            // Write the XML text into the stream
            xmlRequest.Save(writer);
            writer.Close();
            // Send the data to the webserver
            WebResponse rsp = req.GetResponse();
            Stream dataStream = rsp.GetResponseStream();
            // Open the stream using a StreamReader 
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            rsp.Close();
            return responseFromServer;
        }

        public void AddCreditCardDetails(string voultID, string yearMonth, string ccnumber, string cardtype)
        {
            var familyMemberid = _currentUserService.MemberID;
            int month = Convert.ToInt32(yearMonth.Substring(0, 2));
            int year = Convert.ToInt32(yearMonth.Substring(2, 2));           
            var CreditCardfamilyDetails = Repository.Table<CreditCardAccount>().Where(p => p.FamilyMemberID == familyMemberid).FirstOrDefault();

            if (CreditCardfamilyDetails == null)
            {
                var ccDetails = new CreditCardAccount
                {
                    FamilyMemberID = familyMemberid,
                    customer_vault_id = voultID,
                    CardExpirationMonth = month,
                    CardExpirationYear = year,
                    CardStatus = CreditCardStatus.Verified,
                    CardType = (CreditCardType)Enum.Parse(typeof(CreditCardType), cardtype.ToString().Trim()),
                    DateAdded = DateTime.UtcNow,
                    IsCardExpired = false,
                    Isdeleted = false,
                    MaskedCardNumber = ccnumber
                };
                Repository.Insert(ccDetails);
                var CreditCardfamilyDetails1 = Repository.Table<CreditCardAccount>().Where(p => p.FamilyMemberID == familyMemberid).FirstOrDefault();
                var familyIDDetails = Repository.Table<User>().Where(p => p.Id == familyMemberid).FirstOrDefault();
                _signUpProgressService.UpdateSignUpProgress(SignUpStatus.Completed, familyIDDetails.FamilyID);
            }
            else
            {
                CreditCardfamilyDetails.customer_vault_id = voultID;
                CreditCardfamilyDetails.CardExpirationMonth = month;
                CreditCardfamilyDetails.CardExpirationYear = year;
                CreditCardfamilyDetails.DateAdded = DateTime.UtcNow;
                CreditCardfamilyDetails.CardType = (CreditCardType)Enum.Parse(typeof(CreditCardType), cardtype.Trim().ToString());
                CreditCardfamilyDetails.MaskedCardNumber = ccnumber;
                Repository.Update(CreditCardfamilyDetails);
            }

        }
        #endregion
    }
}
