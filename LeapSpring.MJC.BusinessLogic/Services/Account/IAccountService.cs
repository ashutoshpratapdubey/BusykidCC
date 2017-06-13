using LeapSpring.MJC.Core.Domain.Account;
using LeapSpring.MJC.Core.Dto.Accounts;
using System;
using LeapSpring.MJC.Core.Domain.Subscription;
namespace LeapSpring.MJC.BusinessLogic.Services.Account
{
    public interface IAccountService
    {
        /// <summary>
        /// Checks is existing member.
        /// </summary>
        /// <param name="email">The email</param>
        /// <returns><c>True</c>, If the user is already exists. <c>False</c>, Otherwise.</returns>
        bool IsExistingMember(string email);

        /// <summary>
        /// Sign ups a new family.
        /// </summary>
        /// <param name="signUp">The sign up.</param>
        /// <returns>The auth response.</returns>
        AuthResponse SignUp(SignUp signUp);

        /// <summary>
        /// Authenticates the existing user.
        /// </summary>
        /// <param name="login">The login request.</param>
        /// <returns>The auth response.</returns>
        AuthResponse SignIn(LoginRequest login);

        /// <summary>
        /// Login the with pin.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="pin">The pin.</param>
        /// <returns>AuthResponse.</returns>
        AuthResponse LoginWithPin(int memberId, string pin);

        /// <summary>
        /// Retrieves the user pin.
        /// </summary>
        /// <param name="familyMemberId">The family member identifier.</param>
        void RetrievePin(int familyMemberId);

        /// <summary>
        /// Insert the password reset request
        /// </summary>
        /// <param name="emailId">Email id</param>
        /// <returns>The Password reset request</returns>
        PasswordResetRequest PasswordResetRequest(string emailId);

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="password">Password</param>
        /// <returns>The Password reset request</returns>
        PasswordResetRequest ResetPassword(Guid token, string password);

        /// <summary>
        /// Check member is belongs to this current family
        /// </summary>
        /// <param name="familyMemberId">Family member identifier</param>
        /// <returns>Result [bool]</returns>
        bool BelongsToThisFamily(int familyMemberId);

        /// <summary>
        /// Validates the promo code.
        /// </summary>
        /// <param name="promoCode">The promo code.</param>
        /// <returns>The promo code subscription plan.</returns>
        SubscriptionPromoCode ValidatePromoCodeSignUp(string promoCode);

        /// <summary>
        /// Retrieve Pin for Login
        /// </summary>
        /// <param name="FamilyMemberId">The Family Member Id.</param>
        string RetrievePinforLogin(int familyMemberId);

    }
}
