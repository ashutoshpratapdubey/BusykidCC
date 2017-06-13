using LeapSpring.MJC.Core.Domain.Subscription;
using LeapSpring.MJC.Core.Dto;
using LeapSpring.MJC.Core.Enums;
using System;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.SubscriptionService
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Gets the family subscription bt id.
        /// </summary>
        /// <returns>The family subscription.</returns>
        FamilySubscription GetById();

        /// <summary>
        /// Gets the subscription of the family.
        /// </summary>
        /// <returns><c>The subscription status.</returns>
        SubscriptionStatus GetSubscriptionStatus();

        /// <summary>
        /// Subscribe the family
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        void Subscribe(Subscription subscription);

        //string AuthorizeSubscribePlan(Subscription subscription);

        /// <summary>
        /// Renew the annual subscription if expaires.
        /// </summary>
        /// <returns></returns>
        Task RenewSubscription();

        /// <summary>
        /// Cancells the subscription.
        /// </summary>
        Task CancelSubscription();

        /// <summary>
        /// Validates the promo code.
        /// </summary>
        /// <param name="promoCode">The promo code.</param>
        /// <returns>The promo code subscription plan.</returns>
        SubscriptionPromoCode ValidatePromoCode(string promoCode);
    }
}
