using LeapSpring.MJC.Core.Domain.Account;
using LeapSpring.MJC.Core.Dto.Accounts;

namespace LeapSpring.MJC.BusinessLogic.Services.PhoneConfirmation
{
    public interface IPhoneConfirmationService
    {

        /// <summary>
        /// Gets the One Time Password.
        /// </summary>
        /// <param name="phoneNumber">The phone number</param>
        /// <returns>None.</returns>
        void GetVerificationCode(string phoneNumber);

        /// <summary>
        /// Updates the phone number verification.
        /// </summary>
        /// <param name="verificationCode">Verification code</param>
        /// <returns>The Phone Number Confirmation.</returns>
        string VerifyCode(string verificationCode);
    }
}
