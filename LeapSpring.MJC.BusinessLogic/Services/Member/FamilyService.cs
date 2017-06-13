using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using LeapSpring.MJC.BusinessLogic.Services.Cloud;
using LeapSpring.MJC.BusinessLogic.Services.Earnings;
using LeapSpring.MJC.BusinessLogic.Services.Emails;
using LeapSpring.MJC.BusinessLogic.Services.Security;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.Core;
using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Subscription;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LeapSpring.MJC.BusinessLogic.Services.Member
{
    public class FamilyService : ServiceBase, IFamilyService
    {
        private ICryptoService _cryptoService;
        private IStorageService _storageService;
        private IAllocationSettingsService _allocationSettingsService;
        private ICurrentUserService _currentUserService;
        private IEarningsService _earningsService;
        private IAppSettingsService _appSettingsService;
        private ITextMessageService _textMessageService;
        private IEmailTemplateService _emailTemplateService;
        private IEmailService _emailService;
        private IEmailHistoryService _emailHistoryService;
        private ISignUpProgressService _signUpProgressService;
        private ITransactionService _transactionService;
        private IBankService _bankService;
        private ICoreProService _coreproservice;




        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="cryptoService">Crypto service</param>
        /// <param name="storageService">Storage service</param>
        /// <param name="allocationSettingsService">Allocation settings service</param>
        /// <param name="currentUserService">Current user service</param>
        /// <param name="SignUp progress service">Sign up progress service</param>
        public FamilyService(IRepository repository, ICryptoService cryptoService, IStorageService storageService, IAllocationSettingsService allocationSettingsService,
            ICurrentUserService currentUserService, IEarningsService earningsService, IAppSettingsService appSettingsService, ITextMessageService textMessageService,
            IEmailTemplateService emailTemplateService, IEmailService emailService, IEmailHistoryService emailHistoryService,
            ISignUpProgressService signUpProgressService, ITransactionService transactionService, IBankService bankService, ICoreProService CoreProService) : base(repository)
        {
            _cryptoService = cryptoService;
            _storageService = storageService;
            _allocationSettingsService = allocationSettingsService;
            _currentUserService = currentUserService;
            _earningsService = earningsService;
            _appSettingsService = appSettingsService;
            _textMessageService = textMessageService;
            _emailTemplateService = emailTemplateService;
            _emailService = emailService;
            _emailHistoryService = emailHistoryService;
            _signUpProgressService = signUpProgressService;
            _transactionService = transactionService;
            _bankService = bankService;
            _coreproservice = CoreProService;

        }

        #region Private Methods

        /// <summary>
        /// Get encrypt password
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns>The User.</returns>
        private string EncryptPassword(string email, string password, string passwordSalt)
        {
            return _cryptoService.EncryptPassword(email, password, passwordSalt);
        }

        /// <summary>
        /// Get family unique name
        /// </summary>
        /// <param name="familyName">family Name</param>
        /// <returns></returns>
        private string GetFamilyUniqueName(string familyName)
        {
            familyName = familyName.Replace(" ", "");
            var familyNameCount = Repository.Table<Family>().Where(m => m.Name.Equals(familyName) || m.UniqueName.Equals(familyName)).Count();
            if (familyNameCount > 0)
                familyName = familyName + ((familyNameCount - 1) + 1);

            return familyName;
        }

        #endregion

        /// <summary>
        /// Insert new family member
        /// </summary>
        /// <param name="familyMember"></param>
        /// <returns>The family member.</returns>
        public FamilyMember AddMember(FamilyMember familyMember)
        {
            var member = new FamilyMember();
            member.Firstname = familyMember.Firstname;
            member.Lastname = familyMember.Lastname;
            member.DateOfBirth = familyMember.DateOfBirth;
            member.Zipcode = familyMember.Zipcode;
            member.PhoneNumber = familyMember.PhoneNumber.RemoveCountyCode();
            member.ProfileImageUrl = familyMember.ProfileImageUrl;
            member.MemberType = familyMember.MemberType;
            member.Gender = familyMember.Gender;
            member.CreationDate = DateTime.UtcNow;
            member.ProfileStatus = ProfileStatus.InCompleted;
            member.HasTrial = familyMember.HasTrial;
            member.PromoCode = familyMember.PromoCode;

            SignUpStatus signupStatus = SignUpStatus.SingedUp;
            if ((familyMember.MemberType == MemberType.Admin || familyMember.MemberType == MemberType.Parent) && !string.IsNullOrEmpty(familyMember.User?.Email) && !string.IsNullOrEmpty(familyMember.User?.Password))
            {
                var passwordSalt = _cryptoService.GenerateSalt();
                var password = _cryptoService.EncryptPassword(familyMember.User.Email, familyMember.User.Password,
                    passwordSalt);
                familyMember.User.Password = password;
                familyMember.User.PasswordSalt = passwordSalt;
            }

            var user = new User
            {
                Email = familyMember.User?.Email,
                Password = familyMember.User?.Password,
                PIN = familyMember.User?.PIN,
                PasswordSalt = familyMember.User?.PasswordSalt,
                FamilyID = familyMember.User == null ? _currentUserService.FamilyID : familyMember.User.FamilyID,
                Family = (familyMember.User?.FamilyID == 0) ? new Family { Name = familyMember.Lastname, UniqueName = GetFamilyUniqueName(familyMember.Lastname), SignUpStatus = signupStatus } : GetFamilyById(familyMember.User != null ? familyMember.User.FamilyID : _currentUserService.FamilyID),
                LoggedOn = DateTime.UtcNow
            };

            member.User = user;
            Repository.Insert(member);

            // Create new default allocation settings, If child member
            if (member.MemberType == MemberType.Child)
            {
                _allocationSettingsService.CreateNew(member.Id);
                _earningsService.CreateNew(member.Id);
                // Update signup progress
                _signUpProgressService.UpdateSignUpProgress(SignUpStatus.AddedChild);
            }

            return member;
        }

        /// <summary>
        /// Update the family member.
        /// </summary>
        /// <param name="member"></param>
        /// <returns>The family member.</returns>
        public FamilyMember UpdateMember(FamilyMember member)
        {
            var familyMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id.Equals(member.Id) && !p.IsDeleted);
            if (familyMember == null)
                return null;

            var phoneNumber = member.PhoneNumber.RemoveCountyCode();
            var isExistPhoneNumber = Repository.Table<FamilyMember>().Any(p => p.PhoneNumber == phoneNumber && p.Id != member.Id && !p.IsDeleted);
            if (isExistPhoneNumber)
                throw new InvalidParameterException("Entered phone number is already used by another user");

            familyMember.Firstname = member.Firstname;
            familyMember.Lastname = member.Lastname;
            familyMember.DateOfBirth = member.DateOfBirth;
            familyMember.Zipcode = member.Zipcode;
            familyMember.PhoneNumber = phoneNumber;
            familyMember.ProfileImageUrl = member.ProfileImageUrl;
            familyMember.Gender = member.Gender;

            var user = Repository.Table<User>().SingleOrDefault(p => p.Id.Equals(member.UserID));
            user.Email = member.User.Email;
            user.PIN = member.User.PIN;

            if (member.MemberType == MemberType.Admin || member.MemberType == MemberType.Parent)
            {
                var family = Repository.Table<Family>().SingleOrDefault(p => p.Id.Equals(user.FamilyID));
                family.Name = member.Lastname;
                user.Family = family;

                // Create new encrypt password, If user password changed
                if (user.Password != member.User.Password)
                {
                    var newPassword = _cryptoService.EncryptPassword(user.Email, member.User.Password, user.PasswordSalt);
                    if (newPassword != user.Password)
                        user.Password = newPassword;
                }
            }

            familyMember.User = user;

            Repository.Update(familyMember);
            return familyMember;
        }



        /// <summary>
        /// Delete family member by identifier
        /// </summary>
        /// <param name="memberId">Member identifier</param>
        public void DeleteMember(int memberId)
        {
            var familyMember = GetMemberById(memberId);
            if (familyMember.MemberType == MemberType.Child)
            {
                var childEarnings = _earningsService.GetByMemberId(memberId);
                var totalAmount = childEarnings.Save + childEarnings.Share + childEarnings.Spend;
                if (totalAmount > 0)
                {
                    if (!_bankService.IsBankLinked(_currentUserService.MemberID))
                        throw new InvalidOperationException($"Not connected to bank. Please link your bank account to transafer the {familyMember.Firstname}'s money back.");

                    // Tranfer amount to customer account
                    var transactionResult = _transactionService.Transfer(_currentUserService.MemberID, totalAmount, PaymentType.ChildRemoved, TransferType.InternalToExternalAccount);

                    // If transaction failure
                    if (!transactionResult.HasValue)
                        throw new InvalidOperationException("Unable to process the transaction. Please contact bank or mail to us!");

                    childEarnings.Save = 0;
                    childEarnings.Share = 0;
                    childEarnings.Spend = 0;
                    _earningsService.Update(childEarnings);
                }
            }

            familyMember.IsDeleted = true;
            Repository.Update(familyMember);
        }

        /// <summary>
        /// Update member information
        /// </summary>
        /// <param name="dateOfBirth">Date of birth</param>
        /// <param name="address">Address</param>
        /// <param name="city">City</param>
        /// <param name="stateId">State identifier</param>
        /// <param name="ssn">Social security number</param>
        /// <returns>Family member</returns>
        public FamilyMember UpdateMemberInfo(DateTime dateOfBirth, string address, string city, int stateId, string ssn)
        {
            var familyMember = GetMember();

            // Update additional member information
            familyMember.DateOfBirth = dateOfBirth;
            familyMember.Address = address;
            familyMember.City = city;
            familyMember.StateID = stateId;
            familyMember.SSN = ssn;
            Repository.Update(familyMember);
            return familyMember;
        }

        /// <summary>
        /// Gets the family by identifier
        /// </summary>
        /// <returns>The family.</returns>
        public Family GetFamilyById(int familyId)
        {
            var family = Repository.Table<Family>().SingleOrDefault(m => m.Id.Equals(familyId));
            if (family == null)
                throw new InvalidParameterException("Family not found");

            return family;
        }

        /// <summary>
        /// Gets the family member.
        /// </summary>
        /// <returns>The family member.</returns>
        public FamilyMember GetMember()
        {
            var familyMember = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family)
                                .SingleOrDefault(m => m.Id.Equals(_currentUserService.MemberID) && !m.IsDeleted);
            if (familyMember == null)
                throw new InvalidParameterException("Member not found");

            return familyMember;
        }

        public FamilyMember GetChildMember(int id)
        {
            var familyMember = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family)
                                .SingleOrDefault(m => m.Id.Equals(id) && !m.IsDeleted);
            if (familyMember == null)
                throw new InvalidParameterException("Member not found");

            return familyMember;
        }

        /// <summary>
        /// Get admin member
        /// </summary>
        /// <param name="familyId">The family identifier.</param>
        /// <returns>The family member</returns>
        public FamilyMember GetAdmin(int? familyId = null)
        {
            var currentFamilyId = familyId.HasValue ? familyId : _currentUserService.FamilyID;
            var familyMember = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family)
                .FirstOrDefault(m => m.User.FamilyID == currentFamilyId && m.MemberType == MemberType.Admin);
            if (familyMember == null)
                throw new InvalidParameterException("Member not found");

            return familyMember;
        }

        /// <summary>
        /// Gets the family member by id.
        /// </summary>
        /// <param name="familyMemberId"></param>
        /// <returns>The family member.</returns>
        public FamilyMember GetMemberById(int familyMemberId)
        {
            var familyMember = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family)
                                .SingleOrDefault(m => m.Id.Equals(familyMemberId) && !m.IsDeleted);
            if (familyMember == null)
                throw new InvalidParameterException("Invalid family id!");
            return familyMember;
        }

        /// <summary>
        /// Gets all members of the family.
        /// </summary>
        /// <param name="memberType">The member type.</param>
        /// <returns>The list of family members.</returns>
        public List<FamilyMember> GetMembers(MemberType memberType)
        {
            var familyMembers = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family)
                .Where(m => m.User.FamilyID.Equals(_currentUserService.FamilyID) && m.MemberType == memberType && !m.IsDeleted).ToList();
            if (familyMembers == null)
                throw new InvalidParameterException("Invalid family id!");
            return familyMembers;

        }

        /// <summary>
        /// Gets all childrens of the family.
        /// </summary>
        /// <returns>The list of family members.</returns>
        public IList<FamilyMember> GetChildrens()
        {
            return Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family)
                .Where(m => m.User.FamilyID == _currentUserService.FamilyID && m.MemberType == MemberType.Child && !m.IsDeleted).ToList();
        }

        /// <summary>
        /// Upload profile Image
        /// </summary>
        /// <param name="profileImage">The profile image data.</param>
        /// <returns>The image url.</returns>
        public async Task<string> UploadProfileImage(ProfileImage profileImage)
        {
            var familyMember = GetMemberById(profileImage.FamilyMemberId);
            if (familyMember != null && !string.IsNullOrEmpty(familyMember.ProfileImageUrl))
            {
                await _storageService.DeleteFile(familyMember.ProfileImageUrl);
            }

            profileImage.Base64ImageUrl = profileImage.Base64ImageUrl.Substring(profileImage.Base64ImageUrl.IndexOf(',') + 1);
            var fileBytes = Convert.FromBase64String(profileImage.Base64ImageUrl);
            var fileName = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);

            var imageUrl = await _storageService.SaveFile(fileBytes, fileName, profileImage.ContentType);
            familyMember.ProfileImageUrl = imageUrl;
            Repository.Update(familyMember);

            return imageUrl;
        }

        /// <summary>
        /// Update the admin pin number.
        /// </summary>
        /// <param name="pin">Pin number</param>
        /// <returns>The family member.</returns>
        public FamilyMember UpdateAdminPin(string pin)
        {
            // Get family member           
            var familyMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id.Equals(_currentUserService.MemberID));
            if (familyMember == null)
                throw new InvalidParameterException("Invalid Member!");

            // Get user
            var user = Repository.Table<User>().SingleOrDefault(p => p.Id.Equals(familyMember.UserID));
            user.PIN = pin;
            familyMember.User = user;
            familyMember.ProfileStatus = ProfileStatus.Completed;
            Repository.Update(familyMember);
            return familyMember;
        }

        public FamilyMember UpdateParentPin(string pin)
        {
            // Get family member          
            var familyMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id.Equals(_currentUserService.MemberID));
            if (familyMember == null)
                throw new InvalidParameterException("Invalid Member!");

            // Get user
            var user = Repository.Table<User>().SingleOrDefault(p => p.Id.Equals(familyMember.UserID));
            user.PIN = pin;
            familyMember.User = user;
            familyMember.ProfileStatus = ProfileStatus.Completed;

            var PinUpdateTemplate = _emailTemplateService.GetByType(EmailTemplateType.Pinupdated);
            _emailService.Send(user.Email, PinUpdateTemplate.Subject, PinUpdateTemplate.Content);

            Repository.Update(familyMember);
            return familyMember;
        }

        /// <summary>
        /// Update the admin password.
        /// </summary>
        /// <param name="password">password</param>
        /// <returns>The family member.</returns>
        public FamilyMember UpdateAdminPassword(string password)
        {
            // Get family member           
            var familyMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id.Equals(_currentUserService.MemberID));
            if (familyMember == null)
                throw new InvalidParameterException("Invalid Member!");

            // Get user
            var user = Repository.Table<User>().SingleOrDefault(p => p.Id.Equals(familyMember.UserID));

            var newPassword = _cryptoService.EncryptPassword(user.Email, password, familyMember.User.PasswordSalt);
            user.Password = newPassword;
            familyMember.User = user;
            familyMember.ProfileStatus = ProfileStatus.Completed;
            var Passwordtemplate = _emailTemplateService.GetByType(EmailTemplateType.PasswordUpdate);
            _emailService.Send(user.Email, Passwordtemplate.Subject, Passwordtemplate.Content);
            Repository.Update(familyMember);
            return familyMember;
        }



        /// <summary>
        /// Update the member pin number.
        /// </summary>
        /// <param name="pin">Pin number</param>
        /// <param name="memberId">Member identifier</param>
        /// <returns>The family member.</returns>
        public FamilyMember UpdateMemberPin(string pin, int memberId)
        {
            // Get family member
            var familyMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id.Equals(memberId) && !p.IsDeleted);
            if (familyMember == null)
                throw new InvalidParameterException("Invalid Member!");

            // Get user
            var user = Repository.Table<User>().SingleOrDefault(p => p.Id.Equals(familyMember.UserID));
            user.PIN = pin;

            familyMember.User = user;
            familyMember.ProfileStatus = ProfileStatus.Completed;
            Repository.Update(familyMember);
            return familyMember;
        }

        /// <summary>
        /// Updates the admin phonenumber.
        /// </summary>
        /// <param name="phonenumber">The phonenumber.</param>
        /// <returns>FamilyMember.</returns>
        public FamilyMember UpdateAdminPhonenumber(string phonenumber)
        {
            // Get family member
            var familyMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id.Equals(_currentUserService.MemberID));
            if (familyMember == null)
                throw new InvalidParameterException("Invalid Member!");

            phonenumber = phonenumber.RemoveCountyCode();
            var member = Repository.Table<FamilyMember>().Where(p => p.PhoneNumber == phonenumber && !p.IsDeleted);
            if (member.Any())
                throw new InvalidParameterException("Entered phone number is already used by another user");

            familyMember.PhoneNumber = phonenumber;
            Repository.Update(familyMember);
            return familyMember;
        }

        /// <summary>
        /// Updates the child phonenumber.
        /// </summary>
        /// <param name="phonenumber">The phonenumber.</param>
        /// <param name="memberId">The member identifier.</param>
        /// <returns>FamilyMember.</returns>
        public FamilyMember UpdateMemberPhonenumber(string phonenumber, int memberId)
        {
            // Get family member
            var familyMember = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family).SingleOrDefault(p => p.Id.Equals(memberId) && !p.IsDeleted);
            if (familyMember == null)
                throw new InvalidParameterException("Invalid Member!");

            var isNewPhoneNumber = string.IsNullOrEmpty(familyMember.PhoneNumber);

            phonenumber = phonenumber.RemoveCountyCode();
            var member = Repository.Table<FamilyMember>().Where(p => p.PhoneNumber == phonenumber && !p.IsDeleted);
            if (member.Any())
                throw new InvalidParameterException("Entered phone number is already used by another user");

            familyMember.PhoneNumber = phonenumber;
            Repository.Update(familyMember);

            // Send welcome message to child
            if (isNewPhoneNumber && familyMember.MemberType == MemberType.Child)
            {
                var url = HttpContext.Current != null ? HttpContext.Current.Request.UrlReferrer.AbsoluteUri + "#/family/" + familyMember.User?.Family?.UniqueName : string.Empty;

                var message = $"Hello {familyMember.Firstname}, Your parent just signed up for BusyKid, where you can earn money for helping out around the house.  Talk to your parent about getting started!";
                _textMessageService.Send(familyMember.PhoneNumber, message);
            }

            return familyMember;
        }

        /// <summary>
        /// Gets all members of the family.
        /// </summary>
        /// <returns>The list of family members.</returns>
        public Dictionary<MemberType, List<FamilyMember>> GetAllMembers()
        {
            var familyMembers = Repository.Table<FamilyMember>()
                            .Where(m => m.User.FamilyID.Equals(_currentUserService.FamilyID) && !m.IsDeleted).GroupBy(p => p.MemberType).ToDictionary(p => p.Key, p => p.ToList());
            if (familyMembers == null)
                throw new ObjectNotFoundException("Members not found");

            return familyMembers;
        }

        /// <summary>
        /// Get family member by phone number
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <returns>Family member</returns>
        public FamilyMember GetMemberByPhone(string phoneNumber)
        {
            // Get last 10 digit phone number
            phoneNumber = phoneNumber.Substring(phoneNumber.Length - 10);
            return Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family).FirstOrDefault(m => m.PhoneNumber.Contains(phoneNumber) && !m.IsDeleted);
        }

        /// <summary>
        /// Get member by family id and firstname
        /// </summary>
        /// <param name="familyId">Family identifier</param>
        /// <param name="firstname">Firstname</param>
        /// <param name="memberType">Member type</param>
        /// <returns>Family member</returns>
        public FamilyMember GetMember(int familyId, string firstname, MemberType memberType)
        {
            return Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family)
                .FirstOrDefault(m => m.User.FamilyID == familyId && m.Firstname.ToLower().Contains(firstname.ToLower()) && m.MemberType == memberType && !m.IsDeleted);
        }

        /// <summary>
        /// Get child count
        /// </summary>
        /// <returns>Child coun</returns>
        public int GetChildCount()
        {
            return Repository.Table<FamilyMember>().Count(m => m.User.FamilyID == _currentUserService.FamilyID && m.MemberType == MemberType.Child && !m.IsDeleted);
        }

        /// <summary>
        /// Get signup progress
        /// </summary>
        /// <returns>Signup status</returns>
        public SignUpStatus GetSignUpProgress()
        {
            var family = Repository.Table<Family>().Where(m => m.Id == _currentUserService.FamilyID).SingleOrDefault();
            if (family == null)
                throw new InvalidParameterException("Invalid Family!");

            return family.SignUpStatus;
        }

        /// <summary>
        /// Gets all members of the family.
        /// </summary>
        /// <param name="familyName">The family name.</param>
        /// <returns>Family members.</returns>
        //public List<FamilyMember> GetFamilyMembersByName(string familyName)
        //{
        //    var familyMembers = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family)
        //                        .Where(m => m.User.Family.UniqueName.Equals(familyName) && !m.IsDeleted).ToList();
        //    return familyMembers;
        //}
        public List<FamilyMember> GetFamilyMembersByName(string familyName)
        {
            //To stop other to view family and remove sensitive data 03/07/2017
            List<FamilyMember> modfiledList = new List<FamilyMember>();
            var family = Repository.Table<Family>().Where(m => m.Id == _currentUserService.FamilyID).SingleOrDefault();
            if (family.UniqueName.ToUpper() != familyName.ToUpper())
                throw new InvalidParameterException("Invalid Family!");

            var familyMembers = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family)
                              .Where(m => m.User.Family.UniqueName.Equals(familyName) && !m.IsDeleted).ToList();

            ////List<FamilyMember> modfiledList = new List<FamilyMember>();
            foreach (var f in familyMembers)
            {
                var df = (FamilyMember)f;
                df.User.Password = "";
                df.User.PasswordSalt = "";
                df.User.PIN = "";
                modfiledList.Add(df);

            }

            return modfiledList;
        }

        /// <summary>
        ///Update Email
        /// </summary>
        /// <returns>Email</returns>
        public FamilyMember UpdateEmail(string Email, string password, string newemail)
        {
            // Get family member
            var familyMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id.Equals(_currentUserService.MemberID));
            var AdminDetail = Repository.Table<FinancialAccount>().SingleOrDefault(p => p.FamilyMemberID == familyMember.Id);
            var GetCustomerDetail = _coreproservice.get(AdminDetail.CustomerID);


            if (familyMember == null)
                throw new InvalidParameterException("Invalid Member!");

            var user = Repository.Table<User>().SingleOrDefault(p => p.Id.Equals(familyMember.UserID));
            // user.Email = Email;

            var masterPassword = _cryptoService.EncryptPassword(Email, password, familyMember.User.PasswordSalt);

            if (masterPassword == familyMember.User.Password)
            {
                var masterPasswordUpdate = _cryptoService.EncryptPassword(newemail, password, familyMember.User.PasswordSalt);
                familyMember.User.Password = masterPasswordUpdate;
                familyMember.User.Email = newemail;
                familyMember.User = user;

                // update email in corepro account
                var CustomerEmailUpdate = _coreproservice.UpdateEmail(AdminDetail.CustomerID, newemail);

                Repository.Update(familyMember);
            }
            else
            {
                throw new InvalidParameterException("Password doesnot matched!");
            }


            return familyMember;
        }

        /// <summary>
        /// Get states
        /// </summary>
        /// <returns>States</returns>
        public IList<State> GetStates()
        {
            return Repository.Table<State>().ToList();
        }

        /// <summary>
        /// Enables the payday auto approval
        /// </summary>
        /// <param name="adminMember">The admin member of the family</param>
        public void MarkAsPayDayAutoApproval(FamilyMember adminMember)
        {
            var familyMember = Repository.Table<FamilyMember>().SingleOrDefault(p => p.Id == adminMember.Id);
            if (familyMember == null)
                throw new ObjectNotFoundException("Invalid family member!");

            familyMember.PayDayAutoApproval = true;
            Repository.Update(familyMember);
        }

        /// <summary>
        /// Gets the family subscription
        /// </summary>
        /// <param name="familyId">The family identifier.</param>
        /// <returns></returns>
        public FamilySubscription GetFamilySubscription(int? familyId = null)
        {
            familyId = familyId ?? _currentUserService.FamilyID;
            return Repository.Table<Family>()
                .Include(p => p.FamilySubscription)
                .Include(p => p.FamilySubscription.SubscriptionPlan)
                .SingleOrDefault(p => p.Id == familyId && p.FamilySubscriptionID.HasValue)?.FamilySubscription;
        }


        public FamilyMember GetPrePromoCodeStatus(int? memberID = null)
        {
            memberID = memberID ?? _currentUserService.MemberID;
            return Repository.Table<FamilyMember>()
                .SingleOrDefault(p => p.Id == memberID);
        }

        /// <summary>
        /// Updates the family subscription
        /// </summary>
        /// <returns></returns>
        public FamilySubscription UpdatetFamilySubscription(FamilySubscription familySubscription)
        {
            var family = GetFamilyById(_currentUserService.FamilyID);
            family.FamilySubscription = familySubscription;
            Repository.Update(family);
            return family.FamilySubscription;
        }

        /// <summary>
        /// Get incomplete admins
        /// </summary>
        /// <returns>Family members</returns>
        public List<FamilyMember> GetIncompleteAdmins()
        {
            var yesterdayDate = DateTime.UtcNow.AddHours(-24).Date;
            return Repository.Table<FamilyMember>().Include(m => m.User).Where(m => m.MemberType == MemberType.Admin && DbFunctions.TruncateTime(m.CreationDate) > yesterdayDate
                    && m.User.Family.SignUpStatus != SignUpStatus.Completed && !m.IsDeleted).ToList();
        }

        /// <summary>
        /// Get members by last loggedin
        /// </summary>
        /// <param name="memberType">Member type</param>
        /// <param name="loggedOnDate">Logged on date</param>
        /// <returns>Family members</returns>
        public List<FamilyMember> GetMembersByLastLoggedIn(MemberType memberType, DateTime loggedOnDate)
        {
            return Repository.Table<FamilyMember>().Include(m => m.User).Where(m => m.MemberType == memberType && DbFunctions.TruncateTime(m.User.LoggedOn) == loggedOnDate
                        && !m.IsDeleted && m.User.Family.FamilySubscription.Status == SubscriptionStatus.Active).ToList();
        }

        /// <summary>
        /// Get childrens by last loggedin
        /// </summary>
        /// <param name="loggedOnDate">Logged on date</param>
        /// <returns>Family members</returns>
        public List<FamilyMember> GetChildrensByLastLoggedIn(DateTime loggedOnDate)
        {
            var families = Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family)
                .Where(m => m.MemberType == MemberType.Child && DbFunctions.TruncateTime(m.User.LoggedOn) == loggedOnDate && !m.IsDeleted && m.User.Family.FamilySubscription.Status == SubscriptionStatus.Active)
                .GroupBy(p => p.User.FamilyID);

            var admins = new List<FamilyMember>();
            foreach (var members in families)
            {
                var admin = GetAdmin(members.Key); // Key is family identifier
                foreach (var member in members)
                {
                    admins.Add(admin);
                }
            }

            return admins.Distinct().ToList();
        }

        /// <summary>
        /// Gets the upcoming subscriptions.
        /// </summary>
        /// <returns>The list of family member</returns>
        public List<FamilyMember> GetUpcomingSubscriptions()
        {
            var endDate = DateTime.UtcNow.AddDays(3).Date;

            // Gets the name of subscription type
            var subscriptionPlanName = SubscriptionType.Annual.GetEnumDescriptionValue();
            // Gets the annual subscription plan
            var subscriptionPlan = Repository.Table<SubscriptionPlan>().SingleOrDefault(p => p.PlanName.ToLower() == subscriptionPlanName);

            return Repository.Table<FamilyMember>().Include(p => p.User)
                .Where(p => DbFunctions.TruncateTime(p.User.Family.FamilySubscription.EndsOn) == endDate
                && p.User.Family.FamilySubscription.SubscriptionPlan.Id == subscriptionPlan.Id
                && p.User.Family.FamilySubscription.Status == SubscriptionStatus.Active && p.MemberType == MemberType.Admin).ToList();
        }

        /// <summary>
        /// Gets the family by member identifier
        /// </summary>
        /// <returns>The family.</returns>
        public Family GetFamilyByMemberId(int familyMemberId)
        {
            return Repository.Table<FamilyMember>().Include(m => m.User).Include(m => m.User.Family).Include(m => m.User.Family.FamilySubscription).ToList().Where(m => m.Id == familyMemberId && !m.IsDeleted).Select(p => p.User.Family).SingleOrDefault();
        }

        /// <summary>
        /// Toggles the email notification subscription
        /// </summary>
        public void ToggleEmailSubscription()
        {
            var adminMember = GetAdmin();
            if (adminMember == null)
                throw new InvalidParameterException("Invalid Family!");

            adminMember.IsUnSubscribed = !adminMember.IsUnSubscribed;
            Repository.Update(adminMember);
        }

        /// <summary>
        /// Checks that the use has trial.
        /// </summary>
        /// <returns><c>True</c>, If has trial. <c>False</c>, Otherwise.</returns>
        public bool HasTrial()
        {
            var familyMember = GetMember();
            return familyMember.HasTrial;
        }

        /// <summary>
        /// Marks the has trial as used. (i.e. False)
        /// </summary>
        public void MarkTrialAsUsed()
        {
            var familyMember = GetMember();
            if (familyMember == null)
                throw new ObjectNotFoundException("Invalid family member!");

            familyMember.HasTrial = false;
            Repository.Update(familyMember);
        }

        


       



    }
}
