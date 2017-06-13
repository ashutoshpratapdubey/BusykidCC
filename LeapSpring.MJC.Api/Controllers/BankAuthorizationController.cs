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
using System.Xml;
using System.IO;
using LeapSpring.MJC.Core.Domain.Banking;
using System.Configuration;

namespace LeapSpring.MJC.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/bankauthorization")]
    public class BankAuthorizationController : ApiController
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
        public BankAuthorizationController(IBankAuthorizeService bankAuthorizeService, ICurrentUserService currentUserService, IBankService bankService, IFamilyService FamilyService)

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

            if (firstAmount == decimal.Zero || secondAmount == decimal.Zero)
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


        [HttpGet]
        [Route("PrePromocode")]
        public HttpResponseMessage GetPromocode()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _familyService.GetPrePromoCodeStatus());
        }


        [HttpGet]
        [Route("getcreditcard")]
        public HttpResponseMessage GetCreditCard()
        {
            var CreditCard = _bankService.GetCreditCard();
            try
            {
                var CreditCardDetail = new
                {
                    Tokenid = CreditCard.customer_vault_id,
                    Cardtype = CreditCard.CardType,
                    MaskCardNumber = CreditCard.MaskedCardNumber,
                    CardStatus = CreditCard.CardStatus,
                    cardexpirationmonth = CreditCard.CardExpirationMonth,
                    cardexpirationyear = CreditCard.CardExpirationMonth,
                    Iscardexpired = CreditCard.IsCardExpired,
                    dateAdded = CreditCard.DateAdded,
                    isdeleted = CreditCard.Isdeleted

                };
                return Request.CreateResponse(HttpStatusCode.OK, CreditCardDetail);
            }
            catch (Exception ex)
            {
                var CreditCardDetail = new
                {
                    Tokenid = "",
                    Cardtype = "",
                    MaskCardNumber = "",
                    CardStatus = "",
                    cardexpirationmonth = "",
                    cardexpirationyear = "",
                    Iscardexpired = "",
                    dateAdded = "",
                    isdeleted = ""
                };
                return Request.CreateResponse(HttpStatusCode.OK, CreditCardDetail);
            }

        }


        [HttpGet]
        [Route("getcreditcardinfo")]
        public HttpResponseMessage GetCreditCardInfo()
        {
            var CreditCard = _bankService.GetCreditCardInfo();
            try
            {
                var CreditCardDetail = new
                {
                    Tokenid = CreditCard.customer_vault_id,
                    Cardtype = CreditCard.CardType,
                    MaskCardNumber = CreditCard.MaskedCardNumber,
                    CardStatus = CreditCard.CardStatus,
                    cardexpirationmonth = CreditCard.CardExpirationMonth,
                    cardexpirationyear = CreditCard.CardExpirationMonth,
                    Iscardexpired = CreditCard.IsCardExpired,
                    dateAdded = CreditCard.DateAdded,
                    isdeleted = CreditCard.Isdeleted

                };
                return Request.CreateResponse(HttpStatusCode.OK, CreditCardDetail);
            }
            catch (Exception ex)
            {
                var CreditCardDetail = new
                {
                    Tokenid = "",
                    Cardtype = "",
                    MaskCardNumber = "",
                    CardStatus = "",
                    cardexpirationmonth = "",
                    cardexpirationyear = "",
                    Iscardexpired = "",
                    dateAdded = "",
                    isdeleted = ""

                };
                return Request.CreateResponse(HttpStatusCode.OK, CreditCardDetail);
            }

        }

        [HttpGet]
        [Route("changecreditcardstatus")]
        public HttpResponseMessage ChangeCreditCardStatus()
        {
            var UpdateStatus = _bankService.UpdateCardStatus();

            return Request.CreateResponse(HttpStatusCode.OK, UpdateStatus);
        }


        #endregion

        #region Credit Card
        [HttpGet]
        [Route("getcreditcarddetails")]
        public HttpResponseMessage GetCreditCardDetails()
        {
            string xmlResponse = "";
            try
            {
                XmlDocument xmlRequest = new XmlDocument();

                XmlDeclaration xmlDecl = xmlRequest.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                XmlElement root = xmlRequest.DocumentElement;
                xmlRequest.InsertBefore(xmlDecl, root);
                XmlElement xmlSale = xmlRequest.CreateElement("validate");
                XmlElement xmlApiKey = xmlRequest.CreateElement("api-key");
                xmlApiKey.InnerText = ConfigurationManager.AppSettings["CardflexApikey"];
                xmlSale.AppendChild(xmlApiKey);
                XmlElement xmlRedirectUrl = xmlRequest.CreateElement("redirect-url");
                xmlRedirectUrl.InnerText = ConfigurationManager.AppSettings["siteUrl"];
                xmlSale.AppendChild(xmlRedirectUrl);
                XmlElement xmlAmount = xmlRequest.CreateElement("amount");
                xmlAmount.InnerText = "0.00";
                xmlSale.AppendChild(xmlAmount);
                XmlElement xmlAddCustomer = xmlRequest.CreateElement("add-customer");
                XmlElement xmlCustomerVaultId = xmlRequest.CreateElement("customer-vault-id");
                xmlCustomerVaultId.InnerText = "";
                xmlAddCustomer.AppendChild(xmlCustomerVaultId);
                xmlSale.AppendChild(xmlAddCustomer);
                xmlRequest.AppendChild(xmlSale);
                string responseFromServer = _bankService.sendXMLRequest(xmlRequest);
                XmlReader responseReader = XmlReader.Create(new StringReader(responseFromServer));
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(responseReader);
                XmlNodeList response = xDoc.GetElementsByTagName("result");
                if (response[0].InnerText.Equals("1"))
                {
                    XmlNodeList formUrl = xDoc.GetElementsByTagName("form-url");
                    xmlResponse = formUrl[0].InnerText;
                    responseReader.Close();
                }
                else
                {
                    throw new InvalidParameterException("Invalid Credit Card Number.");

                }
                return Request.CreateResponse(HttpStatusCode.OK, xmlResponse);
            }
            catch (Exception ex)
            {
                throw new InvalidParameterException("Invalid Credit Card Number.");
            }

        }

        [HttpGet]
        [Route("validatetokenid/{tokenID}/{cardtype}")]
        public HttpResponseMessage ValidateTokenId(string tokenID, string cardtype)
        {
            string customerVaultId = "";
            string ccexp = "";
            string ccnumber = "";
            XmlDocument xmlRequest = new XmlDocument();
            XmlDeclaration xmlDecl = xmlRequest.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XmlElement root = xmlRequest.DocumentElement;
            xmlRequest.InsertBefore(xmlDecl, root);
            XmlElement xmlCompleteTransaction = xmlRequest.CreateElement("complete-action");
            XmlElement xmlApiKey = xmlRequest.CreateElement("api-key");
            xmlApiKey.InnerText = ConfigurationManager.AppSettings["CardflexApikey"];
            xmlCompleteTransaction.AppendChild(xmlApiKey);
            XmlElement xmlTokenId = xmlRequest.CreateElement("token-id");
            xmlTokenId.InnerText = tokenID;
            xmlCompleteTransaction.AppendChild(xmlTokenId);
            xmlRequest.AppendChild(xmlCompleteTransaction);
            string responseFromServer = _bankService.sendXMLRequest(xmlRequest);
            XmlReader responseReader = XmlReader.Create(new StringReader(responseFromServer));
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(responseReader);
            XmlNodeList response = xDoc.GetElementsByTagName("result");
            XmlNodeList responseText = xDoc.GetElementsByTagName("result-text");
            responseReader.Close();

            if (response[0].InnerText.Equals("1"))
            {
                XmlNodeList VaultId = xDoc.GetElementsByTagName("customer-vault-id");
                XmlNodeList xmlccexp = xDoc.GetElementsByTagName("cc-exp");
                XmlNodeList CCNumber = xDoc.GetElementsByTagName("cc-number");
                customerVaultId = VaultId[0].InnerText;
                ccexp = xmlccexp[0].InnerText;
                ccnumber = (CCNumber[0].InnerText.Length > 3) ? CCNumber[0].InnerText.Substring(CCNumber[0].InnerText.Length - 4, 4) : CCNumber[0].InnerText;
                ccnumber = "************" + ccnumber;
                _bankService.AddCreditCardDetails(customerVaultId, ccexp, ccnumber,cardtype);

            }
            else
            {
                throw new InvalidParameterException("Invalid Credit Card Information.");
            }
             return Request.CreateResponse(HttpStatusCode.OK, true);
        }
        #endregion
    }
}
