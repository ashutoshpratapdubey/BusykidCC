using System.Linq;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Dto.Accounts;
using LeapSpring.MJC.BusinessLogic.Services.Security;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.Data.Repository;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using LeapSpring.MJC.BusinessLogic.Services.SubscriptionService;
using System;
using LeapSpring.MJC.Core.Filters;
using System.Web;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Domain.Chore;
using LeapSpring.MJC.Core.Domain.Banking;
using System.Data.Entity;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.Core.Domain.Account;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.BusinessLogic.Services.Emails;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Core.Domain.Subscription;

namespace LeapSpring.MJC.BusinessLogic.Services.Account
{
    public class AccountService : ServiceBase, IAccountService
    {
        private ICryptoService _cryptoService;
        private IFamilyService _familyService;
        private ICurrentUserService _currentUserService;
        private ITextMessageService _textMessageService;
        private IEmailTemplateService _emailTemplateService;
        private IEmailService _emailService;
        private ISubscriptionService _subscriptionService;
        public AccountService(IRepository repository, ICryptoService cryptoService, IFamilyService familyService, ICurrentUserService currentUserService, ITextMessageService textMessageService,
            IEmailTemplateService emailTemplateService, IEmailService emailService, ISubscriptionService subscriptionService) : base(repository)
        {
            _cryptoService = cryptoService;
            _familyService = familyService;
            _currentUserService = currentUserService;
            _textMessageService = textMessageService;
            _emailTemplateService = emailTemplateService;
            _emailService = emailService;
            _subscriptionService = subscriptionService;
        }

        #region Methods

        /// <summary>
        /// Sign ups a new family.
        /// </summary>
        /// <param name="signUp">The sign up.</param>
        /// <returns>The auth response.</returns>
        public AuthResponse SignUp(SignUp signUp)
        {
            var familyMember = new FamilyMember();
            familyMember.Firstname = signUp.Firstname;
            familyMember.Lastname = signUp.Lastname;
            familyMember.Zipcode = signUp.Zipcode;
            familyMember.MemberType = signUp.MemberType;
            familyMember.HasTrial = signUp.HasTrial;

            if (!string.IsNullOrEmpty(signUp.PromoCode))
            {
                familyMember.PromoCode = signUp.PromoCode.ToUpper();
            }
            else
            {
                familyMember.PromoCode = null;
            }


            if (signUp.MemberType == MemberType.Parent)
                familyMember.PhoneNumber = signUp.PhoneNumber;

            var user = new User { Email = signUp.Email, Password = signUp.Password, FamilyID = signUp.FamilyID.Equals(0) ? 0 : signUp.FamilyID };
            familyMember.User = user;

            var member = _familyService.AddMember(familyMember);


            if (member == null)
                return null;
            var authResponse = new AuthResponse { Firstname = member.Firstname, Lastname = member.Lastname, ProfileUrl = member.ProfileImageUrl, MemberType = member.MemberType, FamilyName = member.User.Family.Name, FamilyId = member.User.FamilyID, FamilyMemberId = member.Id, FamilyUrl = member.User.Family.UniqueName, AccessToken = string.Empty };
            return authResponse;
        }

        /// <summary>
        /// Authenticates the existing user.
        /// </summary>
        /// <param name="login">The login request.</param>
        /// <returns>The auth response.</returns>
        public AuthResponse SignIn(LoginRequest login)
        {
            var user = AuthenticateUser(login.Email, login.Password);
            if (user == null)
                throw new InvalidParameterException("Invalid User Credentials!");

            var member = Repository.Table<FamilyMember>().Where(p => p.UserID == user.Id && !p.IsDeleted).Include(p => p.User).Include(p => p.User.Family).SingleOrDefault();
            if (member == null)
                throw new ObjectNotFoundException("Member not found");

            // Update user loggedon time
            member.User.LoggedOn = DateTime.UtcNow;
            Repository.Update(member.User);

            var authResponse = new AuthResponse { Firstname = member.Firstname, Lastname = member.Lastname, ProfileUrl = member.ProfileImageUrl, MemberType = member.MemberType, FamilyName = member.User.Family.Name, FamilyId = member.User.FamilyID, FamilyMemberId = member.Id, FamilyUrl = member.User.Family.UniqueName, AccessToken = string.Empty };
            return authResponse;
        }

        /// <summary>
        /// Checks is existing member.
        /// </summary>
        /// <param name="email">The email</param>
        /// <returns><c>True</c>, If the user is already exists. <c>False</c>, Otherwise.</returns>
        public bool IsExistingMember(string email)
        {
            return Repository.Table<FamilyMember>().Any(m => m.User.Email == email && !m.IsDeleted);
        }

        /// <summary>
        /// Login the with pin.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="pin">The pin.</param>
        /// <returns>AuthResponse.</returns>
        public AuthResponse LoginWithPin(int memberId, string pin)
        {
            var member = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family).Where(m => m.Id.Equals(memberId) && m.User.PIN.Equals(pin.ToString())
                                && !m.IsDeleted).SingleOrDefault();
            if (member == null)
                throw new ObjectNotFoundException("Invalid PIN!");

            // Update user loggedon time
            member.User.LoggedOn = DateTime.UtcNow;
            Repository.Update(member.User);

            var authResponse = new AuthResponse { Firstname = member.Firstname, Lastname = member.Lastname, ProfileUrl = member.ProfileImageUrl, MemberType = member.MemberType, FamilyName = member.User.Family.Name, FamilyId = member.User.FamilyID, FamilyMemberId = member.Id, FamilyUrl = member.User.Family.UniqueName, AccessToken = string.Empty };
            return authResponse;
        }

        /// <summary>
        /// Retrieves the user pin.
        /// </summary>
        /// <param name="familyMemberId">The family member identifier.</param>
        public void RetrievePin(int familyMemberId)
        {
            var familyMember = _familyService.GetMemberById(familyMemberId);
            if (familyMember == null)
                throw new ObjectNotFoundException("User not found!");

            if (string.IsNullOrEmpty(familyMember.User.PIN))
                throw new ObjectNotFoundException($"You have not finished enrollment for {familyMember.Firstname}!");

            var message = "PIN for " + familyMember.Firstname + " is " + familyMember.User.PIN;

            if (!string.IsNullOrEmpty(familyMember.PhoneNumber))
            {
                _textMessageService.Send(familyMember.PhoneNumber, message);
                return;
            }

            var admin = _familyService.GetAdmin(familyMember.User.FamilyID);
            if (admin == null)
                throw new ObjectNotFoundException("Family not found!");

            if (string.IsNullOrEmpty(admin.PhoneNumber))
                throw new ObjectNotFoundException("Please verify your phone number");

            _textMessageService.Send(admin.PhoneNumber, message);
        }

        /// <summary>
        /// Insert the password reset request
        /// </summary>
        /// <param name="emailId">Email id</param>
        /// <returns>The Password reset request</returns>
        public PasswordResetRequest PasswordResetRequest(string emailId)
        {
            if (string.IsNullOrEmpty(emailId))
                throw new ArgumentNullException("Please provide a valid email");

            var familyMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.User.Email.Equals(emailId) && !p.IsDeleted);
            if (familyMember == null)
                throw new ObjectNotFoundException("We couldn't find your account with that information.");

            var passwordResetRequest = new PasswordResetRequest
            {
                FamilyMemberID = familyMember.Id,
                Token = Guid.NewGuid(),
                Status = PasswordResetStatus.PendingReset,
                CreationDate = DateTime.UtcNow
            };

            Repository.Insert(passwordResetRequest);

            // Email password reset url to parent
            var emailTemplate = _emailTemplateService.GetByType(EmailTemplateType.PasswordReset);
            var bodyContent = emailTemplate?.Content ?? "Password Reseted";

            var resetLink = HttpContext.Current != null ? HttpContext.Current.Request.UrlReferrer.AbsoluteUri + "#/reset/" + passwordResetRequest.Token : string.Empty;
            bodyContent = PrepareTemplateValues(bodyContent, familyMember.Firstname, familyMember.Lastname, resetLink);
            _emailService.Send(emailId, emailTemplate.Subject, bodyContent);

            return passwordResetRequest;
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="password">Password</param>
        /// <returns>The Password reset request</returns>
        public PasswordResetRequest ResetPassword(Guid token, string password)
        {
            if (token == Guid.Empty)
                throw new ArgumentNullException("Please provide a valid token");

            var passwordResetRequest = Repository.Table<PasswordResetRequest>().SingleOrDefault(m => m.Token.Equals(token) && m.Status == PasswordResetStatus.PendingReset);
            if (passwordResetRequest == null)
                throw new InvalidParameterException("Your reset link has already used.");

            if (DateTime.UtcNow > passwordResetRequest.CreationDate.AddHours(24))
                throw new InvalidParameterException("Your reset link has expired.");

            var familyMember = Repository.Table<FamilyMember>().Include(m => m.User).SingleOrDefault(p => p.Id.Equals(passwordResetRequest.FamilyMemberID) && !p.IsDeleted);
            if (familyMember == null)
                throw new ObjectNotFoundException("Member not found.");

            // Create new encrypt password, If user password changed
            var newPassword = _cryptoService.EncryptPassword(familyMember.User.Email, password, familyMember.User.PasswordSalt);
            familyMember.User.Password = newPassword;
            Repository.Update(familyMember.User);

            // Change password reset status
            passwordResetRequest.Status = PasswordResetStatus.Reset;
            Repository.Update(passwordResetRequest);

            return passwordResetRequest;
        }


        /// <summary>
        /// Check member is belongs to this current family
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <returns>Result [bool]</returns>
        public bool BelongsToThisFamily(int familyMemberId)
        {
            var familyId = _currentUserService.FamilyID;
            return Repository.Table<FamilyMember>().Any(m => m.Id == familyMemberId && m.User.FamilyID == familyId && !m.IsDeleted);
        }

        /// <summary>
        /// Retrieves the user pin.
        /// </summary>
        /// <param name="familyMemberId">The family member identifier.</param>
        public string RetrievePinforLogin(int familyMemberId)
        {
            var familyMember = _familyService.GetMemberById(familyMemberId);
            if (familyMember == null)
                throw new ObjectNotFoundException("User not found!");

            if (string.IsNullOrEmpty(familyMember.User.PIN))
                throw new ObjectNotFoundException($"You have not finished enrollment for {familyMember.Firstname}!");

            var message = familyMember.User.PIN;

            return message;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the user by email Id.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns>The User.</returns>
        private User AuthenticateUser(string email, string password)
        {
            var user = Repository.Table<User>().SingleOrDefault(m => m.Email.Equals(email));
            if (user == null)
                return null;

            var masterPassword = _cryptoService.EncryptPassword(email, password, user.PasswordSalt);
            if (!masterPassword.Equals(user.Password))
                return null;
            return user;
        }


        /// <summary>
        /// Validates the promo code.
        /// </summary>
        /// <param name="promoCode">The promo code.</param>
        /// <returns>The promo code subscription plan.</returns>
        public SubscriptionPromoCode ValidatePromoCodeSignUp(string promoCode)
        {
            var subscriptionPromoCode = Repository.Table<SubscriptionPromoCode>().SingleOrDefault(p => p.PromoCode == promoCode && p.IsActive);
            if (subscriptionPromoCode == null)
                // return null;
                throw new ObjectNotFoundException("Invalid promo code!");

            return subscriptionPromoCode;
        }



        #endregion

        #region Utilities

        /// <summary>
        /// Prepare email template values
        /// </summary>
        /// <param name="bodyContent">Email body content</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="resetLink">Reset link</param>
        /// <returns>Body content</returns>
        private string PrepareTemplateValues(string bodyContent, string firstName, string lastName, string resetLink)
        {
            bodyContent = bodyContent.Replace("{{firstname}}", firstName);
            bodyContent = bodyContent.Replace("{{lastname}}", lastName);
            bodyContent = bodyContent.Replace("{{resetLink}}", resetLink);

            return bodyContent;
        }

        #endregion
    }
}
