using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.Core.Dto.Accounts;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Core.Domain.Account;
using LeapSpring.MJC.Core.Enums;
using System.Web;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.BusinessLogic.Services.Banking;

namespace LeapSpring.MJC.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private IAccountService _accountService;
        private ISignUpProgressService _signUpProgressService;
        private ICurrentUserService _currentUserService;
        private ISmsBotService _smsBotService;
        private ICoreProMessageService _coreProMessageService;

        public AccountController(ICoreProMessageService coreProMessageService, IAccountService accountService, ISignUpProgressService signUpProgressService, ICurrentUserService currentUserService, ISmsBotService smsBotService)
        {
            _accountService = accountService;
            _signUpProgressService = signUpProgressService;
            _currentUserService = currentUserService;
            _smsBotService = smsBotService;
            _coreProMessageService = coreProMessageService;
        }

        // POST: api/account/signup
        [HttpPost]
        [Route("signup")]
        public HttpResponseMessage SignUp([FromBody]SignUp signup)
        {
            var isValid = ((signup != null && !string.IsNullOrEmpty(signup.Email) && !string.IsNullOrEmpty(signup.Password)));
            if (!isValid)
                throw new InvalidParameterException("Invalid parameters!");

            signup.Email = signup.Email.ToLower();
            var isExistingUser = _accountService.IsExistingMember(signup.Email);
            if (isExistingUser)
                throw new InvalidParameterException("Email is already taken!");

            if (signup.PromoCode != null)
            {
                if (_accountService.ValidatePromoCodeSignUp(signup.PromoCode).ToString() == null)
                    throw new InvalidParameterException("Invalid promo code");
            }



            var authResponse = _accountService.SignUp(signup);
            if (authResponse == null)
                throw new InvalidOperationException("Cannot able to create the family!");

            authResponse.AccessToken = GetAccessToken(authResponse.FamilyId, authResponse.FamilyMemberId, authResponse.MemberType);



            return Request.CreateResponse(HttpStatusCode.OK, authResponse);




        }


        [HttpPost]
        [Route("signupafterpromocode")]
        public HttpResponseMessage signupafterpromocode([FromBody]SignUp signup)
        {
            signup.PromoCode = null;

            var authResponse = _accountService.SignUp(signup);
            if (authResponse == null)
                throw new InvalidOperationException("Cannot able to create the family!");

            authResponse.AccessToken = GetAccessToken(authResponse.FamilyId, authResponse.FamilyMemberId, authResponse.MemberType);

            return Request.CreateResponse(HttpStatusCode.OK, authResponse);

        }

        // PUT: api/account/signin
        [HttpPut]
        [Route("signin")]
        public HttpResponseMessage SignIn([FromBody]LoginRequest login)
        {

            if (login == null && string.IsNullOrEmpty(login.Email) && string.IsNullOrEmpty(login.Password))
                throw new InvalidParameterException("Invalid parameters!");

            login.Email = login.Email.ToLower();
            var authResponse = _accountService.SignIn(login);
            if (authResponse == null)
                throw new ObjectNotFoundException("Family not found!");

            authResponse.AccessToken = GetAccessToken(authResponse.FamilyId, authResponse.FamilyMemberId, authResponse.MemberType);
            return Request.CreateResponse(HttpStatusCode.OK, authResponse);
        }

        // GET: api/account/getsignupprogress
        [Authorize]
        [HttpGet]
        [Route("getdetailedsignupprogress")]
        public HttpResponseMessage GetDetailedSignUpProgress()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _signUpProgressService.GetDetailedSignUpProgress());
        }

        [HttpPut]
        [Route("loginwithpin/{memberId}/{pin}")]
        public HttpResponseMessage LoginWithPin(int memberId, string pin)
        {
            var authResponse = _accountService.LoginWithPin(memberId, pin);
            //added by STPL
            var familyID = _currentUserService.FamilyID;
            if (familyID != authResponse.FamilyId)
                throw new InvalidParameterException("Invalid Authorization!");

            authResponse.AccessToken = GetAccessToken(authResponse.FamilyId, authResponse.FamilyMemberId, authResponse.MemberType);
            return Request.CreateResponse(HttpStatusCode.OK, authResponse);
        }

        // api/account/retrievepin/{0}
        [HttpGet]
        [Route("retrievepin/{memberId}")]
        public void RetrievePin(int memberId)
        {
            if (memberId == 0)
                throw new InvalidParameterException("Invalid parameters!");

            _accountService.RetrievePin(memberId);
        }

        [HttpPut]
        [Route("passwordresetrequest")]
        public PasswordResetRequest PasswordResetRequest(string emailId)
        {
            return _accountService.PasswordResetRequest(emailId.ToLower());
        }

        [HttpPut]
        [Route("resetpassword/{token:Guid}/{password}")]
        public PasswordResetRequest ResetPassword(Guid token, string password)
        {
            return _accountService.ResetPassword(token, password);
        }

        [HttpPut]
        [Route("LoginforDashboard/{memberId}/{memberType}/{UniqueName}")]
        public HttpResponseMessage LoginforDashboard(int memberId, string MemberType, string UniqueName)
        {
            var MemberPin = _accountService.RetrievePinforLogin(memberId);

            var authResponse = _accountService.LoginWithPin(memberId, MemberPin);
            //added by STPL
            var familyID = _currentUserService.FamilyID;
            if (familyID != authResponse.FamilyId)
                throw new InvalidParameterException("Invalid Authorization!");

            authResponse.AccessToken = GetAccessToken(authResponse.FamilyId, authResponse.FamilyMemberId, authResponse.MemberType);
            return Request.CreateResponse(HttpStatusCode.OK, authResponse);
        }


        [HttpGet]
        [Route("validatepromocode/{promocode}")]
        public HttpResponseMessage ValidatePromo(string promocode)
        {
            var Promocodevalue = _accountService.ValidatePromoCodeSignUp(promocode);
            if (Promocodevalue == null)
                throw new InvalidParameterException("Invalid Promo code!");

            return Request.CreateResponse(HttpStatusCode.OK, Promocodevalue);
        }


        #region Methods

        /// <summary>
        /// Get authentication ticket to generate access token.
        /// </summary>
        /// <param name="familyId">The family identifier.</param>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <param name="memberType">The member type.</param>
        /// <returns>The authentication ticket.</returns>
        private string GetAccessToken(int familyId, int familyMemberId, MemberType memberType)
        {
            var identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);
            identity.AddClaim(new Claim("FamilyID", (familyId == 0) ? string.Empty : familyId.ToString()));
            identity.AddClaim(new Claim("MemberID", (familyMemberId == 0) ? string.Empty : familyMemberId.ToString()));
            identity.AddClaim(new Claim("MemberType", ((int)memberType).ToString()));

            var authenticationTicket = new AuthenticationTicket(identity, new AuthenticationProperties());
            var currentUtc = new SystemClock().UtcNow;
            authenticationTicket.Properties.IssuedUtc = currentUtc;
            authenticationTicket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromDays(2));
            return Startup.OAuthBearerOptions.AccessTokenFormat.Protect(authenticationTicket);
        }

        [HttpGet]
        [Route("getmobileresponse/{OneValue}/{TwoValue}")]
        public void getmobileresponse(string OneValue, string TwoValue)
        {
            //var twilioResponse = new TwilioResponse();
            _smsBotService.ReceiveDummy(OneValue, TwoValue);
            //twilioResponse.Sms("Test"); // Todo: Reply here

        }

        [HttpGet]
        [Route("getcoreproresponse")]
        public void getcoreproresponse()
        {
            //var twilioResponse = new TwilioResponse();
            //_smsBotService.Receive(smsResponse);
            //twilioResponse.Sms("Test"); // Todo: Reply here
            _coreProMessageService.ReceiveMessage();
        }
        #endregion

    }






}
