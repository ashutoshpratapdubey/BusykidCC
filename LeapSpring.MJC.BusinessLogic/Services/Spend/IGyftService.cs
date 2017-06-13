using LeapSpring.MJC.Core.Dto.Spend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Spend
{
    /// <summary>
    /// Represents a interface of gyft api service
    /// </summary>
    public interface IGyftService
    {
        /// <summary>
        /// Get gift cards from gyft api
        /// </summary>
        /// <returns>Gift cards</returns>
        Task<IList<GyftCardItem>> GetGiftCards();

        /// <summary>
        /// Purchase gift card
        /// </summary>
        /// <param name="giftPurchaseRequest">Gift purchase request</param>
        /// <returns>Gift card url</returns>
        Task<string> PurchaseGiftCard(GyftPurchaseRequest giftPurchaseRequest);
    }
}
