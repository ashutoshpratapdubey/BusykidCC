using LeapSpring.MJC.Core.Dto.Save;
using LeapSpring.MJC.Core.Dto.Save.StockPilePurchase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Save
{
    public interface IStockPileService
    {
        /// <summary>
        /// Gets the stock gift items.
        /// </summary>
        /// <returns>The list of stock gift items.</returns>
        Task<IList<StockGiftItem>> GetStockGiftItems();

        /// <summary>
        /// Gets the stock gift quotes.
        /// </summary>
        /// <returns>The list of gift stock quotes.</returns>
        Task<IList<GiftStockQuote>> GetStockGiftQuotes();

        /// <summary>
        /// Purchase stock gift card.
        /// </summary>
        /// <param name="purchaseStock">The purchase request data</param>
        /// <returns>The purchase stock response.</returns>
        Task<PurchaseStockResponse> PurchaseStock(PurchaseStockRequest purchaseStock);

        /// <summary>
        /// Order the stock purchase.
        /// </summary>
        /// <param name="transactionId">The transaction identifier</param>
        /// <returns>The stock order response which contains redeem url.</returns>
        Task<OrderResponse> Order(string transactionId);
    }
}
