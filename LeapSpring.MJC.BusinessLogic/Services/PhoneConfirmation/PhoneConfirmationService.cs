using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.Core.Domain.Account;
using LeapSpring.MJC.Core.Dto.Accounts;
using LeapSpring.MJC.Core.Enums;
using LeapSpring.MJC.Data.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Filters;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.Core;

namespace LeapSpring.MJC.BusinessLogic.Services.PhoneConfirmation
{
    public class PhoneConfirmationService : ServiceBase, IPhoneConfirmationService
    {
        private ITextMessageService _textMessageService;
        private IAppSettingsService _appSettingsService;
        ICurrentUserService _currentUserService;

        public PhoneConfirmationService(IRepository repository, ITextMessageService textMessageService, IAppSettingsService appSettingsService, ICurrentUserService currentUserService) : base(repository)
        {
            _textMessageService = textMessageService;
            _appSettingsService = appSettingsService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Gets the One Time Password.
        /// </summary>
        /// <param name="phoneNumber">The phone number</param>
        /// <returns>The Phone Number Confirmation.</returns>
        public void GetVerificationCode(string phoneNumber)
        {
            phoneNumber = phoneNumber.RemoveCountyCode();
            var familyMember = Repository.Table<FamilyMember>().Where(p => p.PhoneNumber == phoneNumber);

            if (familyMember.Any())
                throw new InvalidParameterException("Entered phone number is already used by another user");

            ResetOTP();

            var phoneNumberConfirmation = new PhoneNumberConfirmation
            {
                VerificationCode = GenerateOTP(),
                CreatedTime = DateTime.UtcNow,
                Status = VerificationCodeStatus.Active,
                FamilyMemberID = _currentUserService.MemberID,
            };

            Repository.Insert(phoneNumberConfirmation);

            phoneNumberConfirmation = Repository.Table<PhoneNumberConfirmation>().Include(p => p.FamilyMember).SingleOrDefault(p => p.Id.Equals(phoneNumberConfirmation.Id));

            var message = $"Your confirmation code is {phoneNumberConfirmation.VerificationCode}";
            _textMessageService.Send(phoneNumber, message);
        }

        /// <summary>
        /// Updates the phone number verification.
        /// </summary>
        /// <param name="verificationCode">Verification code</param>
        /// <returns>The Phone Number Confirmation.</returns>
        public string VerifyCode(string verificationCode)
        {
            var phoneNumberConfirmation = Repository.Table<PhoneNumberConfirmation>().SingleOrDefault(p => p.FamilyMemberID.Equals(_currentUserService.MemberID) && p.VerificationCode.Equals(verificationCode) && p.Status.ToString().Equals(VerificationCodeStatus.Active.ToString()));
            if (phoneNumberConfirmation == null)
                return "Invalid verification code";

            if (!verificationCode.Equals(phoneNumberConfirmation.VerificationCode))
                return "Invalid verification code";

            if (phoneNumberConfirmation.Status.ToString().Equals(VerificationCodeStatus.Used.ToString()))
                return "Entered verification code is already used";

            if (phoneNumberConfirmation.Status.ToString().Equals(VerificationCodeStatus.Expired.ToString()))
                return "Your verification code has expired";

            phoneNumberConfirmation.Status = DateTime.UtcNow > phoneNumberConfirmation.CreatedTime.AddMinutes(3) ? VerificationCodeStatus.Expired : VerificationCodeStatus.Used;

            Repository.Update(phoneNumberConfirmation);

            return (phoneNumberConfirmation.Status.ToString().Equals(VerificationCodeStatus.Expired.ToString())) ? "Your verification code has expired" : "Verified";
        }


        #region Private Methods

        /// <summary>
        /// Generates the One Time Password.
        /// </summary>
        /// <returns>The One Time Password.</returns>
        private string GenerateOTP()
        {
            var characters = _appSettingsService.OtpCharacters.ToCharArray();
            var OneTimePassword = string.Empty;
            var rand = new Random();
            for (int i = 0; i < 6; i++)
            {
                //It will not allow Repetation of Characters
                int position = rand.Next(1, characters.Length);
                if (!OneTimePassword.Contains(characters.GetValue(position).ToString()))
                    OneTimePassword += characters.GetValue(position);
                else
                    i--;
            }
            return OneTimePassword;
        }

        /// <summary>
        /// Reset all Otp to Expairy state
        /// </summary>
        private void ResetOTP()
        {
            var phoneNumberConfirmations = Repository.Table<PhoneNumberConfirmation>().Where(p => p.FamilyMemberID.Equals(_currentUserService.MemberID) && (p.Status.ToString().Equals(VerificationCodeStatus.Active.ToString()))).ToList();

            foreach (var phoneNumberConfirmation in phoneNumberConfirmations)
            {
                phoneNumberConfirmation.Status = VerificationCodeStatus.Expired;
                Repository.Update(phoneNumberConfirmation);
            }
        }

        #endregion

    }
}
