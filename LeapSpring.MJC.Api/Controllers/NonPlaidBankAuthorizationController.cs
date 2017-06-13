using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using LeapSpring.MJC.Core.Dto.Banking;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using LeapSpring.MJC.BusinessLogic.Services.Member;

namespace LeapSpring.MJC.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/nonplaidbankauthorization")]
    public class NonPlaidBankAuthorizationController : ApiController
    {
        #region Fields

        private readonly IBankAuthorizeService _bankAuthorizeService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBankService _bankService;
        private readonly IFamilyService _familyService;

        #endregion

        #region Ctor

        /// <summary>
        /// </summary>
        /// <param name="bankAuthorizeService">Bank authorize service</param>
        /// <param name="currentUserService">Current user service</param>
        /// <param name="bankService">Bank service</param>
        public NonPlaidBankAuthorizationController(IBankAuthorizeService bankAuthorizeService, ICurrentUserService currentUserService, IBankService bankService, IFamilyService FamilyService)

        {
            _bankAuthorizeService = bankAuthorizeService;
            _currentUserService = currentUserService;
            _bankService = bankService;
            _familyService = FamilyService;
        }

        #endregion

        #region Corepro

        // PUT: api/bankauthorization/createcustomer
        [HttpPut]
        [Route("createcustomer")]
        public void CreateCustomer()
        {
            if (_currentUserService.MemberType != MemberType.Admin)
                throw new UnauthorizedAccessException();

            _bankAuthorizeService.CreateCustomer();
        }

        // PUT: api/bankauthorization/linkbankaccount
        [HttpPut]
        [Route("linkbankaccount")]
        public async Task LinkBankAccount(string publicToken, string institutionName, string selectedAccountId)
        {
            if (_currentUserService.MemberType != MemberType.Admin)
                throw new UnauthorizedAccessException();

            if (string.IsNullOrEmpty(publicToken))
                throw new InvalidParameterException("Invalid public token!");

            await _bankAuthorizeService.LinkBankAccount(publicToken, institutionName, selectedAccountId);
        }

        // PUT: api/bankauthorization/linkmicrodepositaccount?accountNumber=&routingNumber=&accountType=
        [HttpPut]
        [Route("linkmicrodepositaccount")]
        public async Task LinkMicroDepositAccount(string accountNumber, string routingNumber, string accountType)
        {
            if (_currentUserService.MemberType != MemberType.Admin)
                throw new UnauthorizedAccessException();

            if (string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(routingNumber) || string.IsNullOrEmpty(accountType))
                throw new InvalidParameterException("Invalid parameter!");

            await _bankAuthorizeService.LinkMicroDepositAccount(accountNumber, routingNumber, accountType);
        }

        // GET: api/bankauthorization/verify?firstAmount=&secondAmount=
        [HttpPut]
        [Route("verify")]
        public async Task Verify(decimal firstAmount, decimal secondAmount)
        {
            if (_currentUserService.MemberType != MemberType.Admin)
                throw new UnauthorizedAccessException();

            if (firstAmount == decimal.Zero)
                throw new InvalidParameterException("Invalid parameters!");

            await _bankAuthorizeService.VerifyBankAccount(firstAmount, secondAmount);
        }

        // PUT: api/bankauthorization/removebank
        [HttpDelete]
        [Route("removebank")]
        public async Task RemoveBank()
        {
            if (_currentUserService.MemberType != MemberType.Admin)
                throw new UnauthorizedAccessException();

            await _bankService.RemoveBank();
        }

        // GET: api/bankauthorization/getfinancialaccount
        [HttpGet]
        [Route("getfinancialaccount")]
        public HttpResponseMessage GetFinancialAccount()
        {
            if (_currentUserService.MemberType != MemberType.Admin)
                throw new UnauthorizedAccessException();

            return Request.CreateResponse(HttpStatusCode.OK, _bankService.GetFinancialAccount());
        }

        // GET: api/bankauthorization/getbankdocuments
        [HttpGet]
        [Route("getbankdocuments")]
        public HttpResponseMessage GetBankDocuments()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _bankAuthorizeService.GetBankDocuments());
        }

        // GET: api/bankauthorization/getbankdocumentbyid/{documentId}
        [HttpGet]
        [Route("getbankdocumentbyid/{documentId}")]
        public HttpResponseMessage GetBankDocumentById(int documentId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _bankAuthorizeService.GetBankDocumentById(documentId));
        }

        // GET: api/bankauthorization/getbankdocuments
        [HttpGet]
        [Route("getlinkedbankstatus")]
        public HttpResponseMessage GetLinkedBankStatus()
            {
            var financialAccount = _bankService.GetFinancialAccount();

            var bankStatus = new
            {
                BankName = financialAccount?.BankName,
                AccountNumber = financialAccount?.MaskedAccountNumber,
                IsLinkedBank = financialAccount?.ExternalAccountID.HasValue,
                BankStatus = financialAccount?.Status,
                AccountType = financialAccount?.AccountType,
                IsPlaidAccount= financialAccount?.PlaidAccessToken != null ? 1: 0,
                AccountId = financialAccount?.AccountID //Added by prachi 06may17
            };

            return Request.CreateResponse(HttpStatusCode.OK, bankStatus);
        }

        // GET: api/bankauthorization/isbanklinked
        [HttpGet]
        [Route("isbanklinked")]
        public HttpResponseMessage IsBankLinked()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _bankService.IsBankLinked());
        }

        #endregion

        // GET: api/bankauthorization/isbanklinked
        [HttpGet]
        [Route("PrePromocode")]
        public HttpResponseMessage GetPromocode()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _familyService.GetPrePromoCodeStatus());
        }
    }
}
