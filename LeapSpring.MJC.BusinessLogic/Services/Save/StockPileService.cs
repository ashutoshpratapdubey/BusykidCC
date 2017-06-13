using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.Core.Dto.Save;
using LeapSpring.MJC.Core.Dto.Save.StockPilePurchase;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LeapSpring.MJC.BusinessLogic.Services.Banking;

namespace LeapSpring.MJC.BusinessLogic.Services.Save
{
    public class StockPileService : IStockPileService
    {
        private IAppSettingsService _appSettingsService;
        private ITransactionService _transactionService;

        public StockPileService(IAppSettingsService appSettingsService, ITransactionService transactionService)
        {
            _appSettingsService = appSettingsService;
            _transactionService = transactionService;
        }

        #region Utilities

        async private Task<HttpResponseMessage> CallApi(string endPoint, string methodName, string postData = "")
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                try
                {
                    string fullURLString = string.Format("{0}{1}", _appSettingsService.StockPileApiBaseUrl, endPoint);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("SPAccessID", _appSettingsService.StockPileAccessID);
                    client.DefaultRequestHeaders.Add("SPAccessSecret", _appSettingsService.StockPileAccessSecret);

                    if (WebRequestMethods.Http.Get == methodName)
                        response = await client.GetAsync(fullURLString);
                    else
                        response = await client.PostAsync(fullURLString, new StringContent(postData, Encoding.UTF8, "application/json"));

                    if (!response.IsSuccessStatusCode)
                    {
                        // Throw exeption if not a success code.
                        var errorResponse = response.Content.ReadAsStringAsync().Result;
                        throw new HttpException((int)response.StatusCode, errorResponse);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }

                return response;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the stock gift items.
        /// </summary>
        /// <returns>The list of stock gift items.</returns>
        public async Task<IList<StockGiftItem>> GetStockGiftItems()
        {
            var response = await CallApi(_appSettingsService.StockGiftItemsEndPoint, WebRequestMethods.Http.Get);

            if (response != null)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var stockGiftItemResponse = JsonConvert.DeserializeObject<StockGiftItemResponse>(responseString);
                var test = stockGiftItemResponse.StockItems.Where(p => p.Categories != null && p.Categories.Contains("Kids")).Take(20).ToList();
                return test;
            }
            return null;
        }

        /// <summary>
        /// Gets the stock gift quotes.
        /// </summary>
        /// <returns>The list of gift stock quotes.</returns>
        public async Task<IList<GiftStockQuote>> GetStockGiftQuotes()
        {
            var response = await CallApi(_appSettingsService.StockGiftQuotesEndPoint, WebRequestMethods.Http.Get);

            if (response != null)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var stockGiftQuoteResponse = JsonConvert.DeserializeObject<StockGiftQuoteResponse>(responseString);
                return stockGiftQuoteResponse.StockGiftQuotes;
            }
            return null;
        }

        /// <summary>
        /// Purchase stock gift card.
        /// </summary>
        /// <param name="purchaseStock">The purchase request data</param>
        /// <returns>The purchase stock response.</returns>
        public async Task<PurchaseStockResponse> PurchaseStock(PurchaseStockRequest purchaseStock)
        {
            var purchaseRequestData = JsonConvert.SerializeObject(purchaseStock);
            var response = await CallApi(_appSettingsService.StockPilePurchaseEndPoint, WebRequestMethods.Http.Post, purchaseRequestData);
            if (response != null)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var purchaseStockResponse = JsonConvert.DeserializeObject<PurchaseStockResponse>(responseString);
                if (purchaseStockResponse.Status != "SUCCESS")
                    throw new InvalidOperationException(purchaseStockResponse.ErrorDetail);
                return purchaseStockResponse;
            }
            return null;
        }

        /// <summary>
        /// Order the stock purchase.
        /// </summary>
        /// <param name="transactionId">The transaction identifier</param>
        /// <returns>The stock order response which contains redeem url.</returns>
        public async Task<OrderResponse> Order(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                throw new InvalidOperationException("Invalid transaction!");

            var orderEndPoint = string.Format(_appSettingsService.OrderStockEndPoint, transactionId);
            var response = await CallApi(orderEndPoint, WebRequestMethods.Http.Get);
            if (response != null)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var orderResponse = JsonConvert.DeserializeObject<OrderResponse>(responseString);
                if (orderResponse.Status != "SUCCESS")
                    throw new InvalidOperationException(orderResponse.ErrorCode);
                return orderResponse;
            }
            return null;
        }
        
        #endregion
    }
}
