using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Spend;
using LeapSpring.MJC.Core.Dto.Spend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Spend
{
    /// <summary>
    /// Represents a interface of spend service
    /// </summary>
    public interface ISpendService
    {
        /// <summary>
        /// Add purchased gift card
        /// </summary>
        /// <param name="purchasedGiftCard">Purchased gift card</param>
        void AddPurchasedGiftCard(PurchasedGiftCard purchasedGiftCard);

        /// <summary>
        /// Update purchased gift card
        /// </summary>
        /// <param name="purchasedGiftCard">Purchased gift card</param>
        void UpdatePurchasedGiftCard(PurchasedGiftCard purchasedGiftCard);

        /// <summary>
        /// Get purchased gift card by identifier
        /// </summary>
        /// <param name="purchasedGiftCardId">Purchased gift card by identifier</param>
        /// <returns>Purchased gift card</returns>
        PurchasedGiftCard GetPurchasedGiftCardById(int purchasedGiftCardId);

        /// <summary>
        /// Delete purchased gift card by identifier
        /// </summary>
        /// <param name="purchasedGiftCardId">Purchased gift card by identifier</param>
        void DeletePurchasedGiftCard(int purchasedGiftCardId);

        /// <summary>
        /// Get purchased gift cards 
        /// </summary>
        /// <returns>Purchased gift cards</returns>
        IList<PurchasedGiftCard> GetPurchasedGiftCards();

        /// <summary>
        /// Get gift cards
        /// </summary>
        /// <returns>Gift cards</returns>
        IList<GiftCard> GetGiftCards();

        /// <summary>
        /// Get gift cards grouped by merchant name
        /// </summary>
        /// <param name="isFeatured">Is featured</param>
        /// <returns>Gift cards</returns>
        IList<GiftCardPreview> GetGiftCardPreviews(bool isFeatured);

        /// <summary>
        /// Purchase gift card request
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <returns>Purchased gift card</returns>
        PurchasedGiftCard PurchaseGiftCardRequest(int giftCardId);

        /// <summary>
        /// Initiates the cash out request.
        /// </summary>
        /// <param name="cashOutRequest">The cashout request.</param>
        /// <param name="familyMemberId">The family member identifier.</param>
        /// <returns>The cashout.</returns>
        CashOut CashOutRequest(CashOut cashOutRequest, int? familyMemberId = null);

        /// <summary>
        /// Approves the cash out request.
        /// </summary>
        /// <param name="adminMember">The admin member of the family.</param>
        /// <param name="pendingCashOutId">The pending cash out identifier.</param>
        /// <returns></returns>
        void ApproveCashOut(FamilyMember adminMember, int pendingCashOutId);

        /// <summary>
        /// Disapproves the pending cash out.
        /// </summary>
        /// <param name="pendingCashOutId">The pending cash out identifier.</param>
        /// <returns>/returns>
        void DisapproveCashOut(int pendingCashOutId);

        /// <summary>
        /// Approve purchase gift card 
        /// </summary>
        /// <param name="adminMember">Admin member</param>
        /// <param name="purchasedGiftCardId">Purchased gift card identifier</param>
        /// <returns></returns>
        Task ApprovePurchaseGiftCard(FamilyMember adminMember, int purchasedGiftCardId);

        /// <summary>
        /// Disapproves the purchased gift card 
        /// </summary>
        /// <param name="purchasedGiftCardId">Purchased gift card identifier</param>
        /// <returns></returns>
        void DisapprovePurchasedGiftCard(int purchasedGiftCardId);
    }
}
