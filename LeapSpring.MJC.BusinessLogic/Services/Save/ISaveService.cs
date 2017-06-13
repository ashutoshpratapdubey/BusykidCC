using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Save;
using LeapSpring.MJC.Core.Dto.Save;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Save
{
    public interface ISaveService
    {
        /// <summary>
        /// Gets the stock gift card.
        /// </summary>
        /// <returns>The stock gift card.</returns>
        StockItem GetById(int id);

        /// <summary>
        /// Gets the stock gift cards.
        /// </summary>
        /// <param name="isFeaturedStock">The is featured stock.</param>
        /// <returns>The list of stock gift cards.</returns>
        IList<StockItem> GetStockGiftCards(bool isFeaturedStock);

        /// <summary>
        /// Gets the purchased stock gift cards
        /// </summary>
        /// <returns></returns>
        IQueryable<StockPurchaseRequest> GetPurchasedStockGiftCards();

        /// <summary>
        /// Gets the disapproved stock gift cards
        /// </summary>
        /// <returns>The disapproved stocks</returns>
        IQueryable<StockPurchaseRequest> GetDisapprovedStockGiftCards();

        /// <summary>
        /// Gets and updates the stock quotes.
        /// </summary>
        /// <returns>The list of stock quotes.</returns>
        Task<IList<GiftStockQuote>> GetStockGiftQuotes();

        /// <summary>
        /// Update the stock quotes.
        /// </summary>
        /// <returns></returns>
        Task UpdateStockGiftQuotes();

        /// <summary>
        /// initiates the stock purchase.
        /// </summary>
        /// <param name="stockPurchaseRequest">The stock purchase request.</param>
        /// <returns>The stock purchase request.</returns>
        StockPurchaseRequest InitiateStockPurchase(StockPurchaseRequest stockPurchaseRequest);

        /// <summary>
        /// Aproves the stock purchase.
        /// </summary>
        /// <param name="adminMember">Admin member</param>
        /// <param name="pendingStockRequestId">Pending stock request identifier.</param>
        /// <returns></returns>
        Task ApproveStockPurchase(FamilyMember adminMember, int pendingStockRequestId);

        /// <summary>
        /// Disapproves the purchased stock.
        /// </summary>
        /// <param name="purchasedStockId">The purchased stock identifier.</param>
        /// <returns></returns>
        void DisapprovePurchasedStock(int purchasedStockId);
    }
}
