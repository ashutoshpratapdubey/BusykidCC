using LeapSpring.MJC.Data.Repository;
using System;
using System.Net;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using LeapSpring.MJC.Core.Dto.Spend;
using Newtonsoft.Json;

namespace LeapSpring.MJC.BusinessLogic.Services.Spend
{
    /// <summary>
    /// Represents a gyft api service
    /// </summary>
    public class GyftService : ServiceBase, IGyftService
    {
        private string _gyftApiBaseUrl;
        private string _gyftApiKey;
        private string _gyftApiSecret;
        private readonly string _shopCardUrl = "/reseller/shop_cards";
        private readonly string _purchaseCardUrl = "/partner/purchase/gift_card_direct";

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="repository">Repository</param>
        public GyftService(IRepository repository) : base(repository)
        {
            _gyftApiBaseUrl = ConfigurationManager.AppSettings["GyftBaseUrl"];
            _gyftApiKey = ConfigurationManager.AppSettings["GyftKey"];
            _gyftApiSecret = ConfigurationManager.AppSettings["GyftSecret"];
        }

        #region Utilities

        async private Task<HttpResponseMessage> CallApi(string url, string methodName, string postData = "")
        {
            HttpResponseMessage response;
            TimeSpan timeStan = DateTime.UtcNow - new DateTime(1970, 1, 1);
            string timestamp = ((int)timeStan.TotalSeconds).ToString();
            using (var client = new HttpClient())
            {
                try
                {
                    var signature = GenerateSignature();
                    string fullURLString = string.Format("{0}&sig={2}", url, _gyftApiKey, signature);

                    // HTTP POST
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("x-sig-timestamp", timestamp);

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
                catch
                {
                    return null;
                    //throw new InvalidOperationException(ex.Message);
                }

                return response;
            }
        }


        /// <summary>
        /// Generate signature code
        /// </summary>
        /// <returns>Signature code</returns>
        private string GenerateSignature()
        {
            TimeSpan timeStan = DateTime.UtcNow - new DateTime(1970, 1, 1);
            string timestamp = ((int)timeStan.TotalSeconds).ToString();
            string stringToSign = _gyftApiKey + _gyftApiSecret + timestamp;
            SHA256 sha256 = SHA256Managed.Create();
            byte[] messageBytes = Encoding.UTF8.GetBytes(stringToSign);
            byte[] hash = sha256.ComputeHash(messageBytes);
            return HexEncode(hash);
        }

        /// <summary>
        /// Convert hexadecimal encode
        /// </summary>
        /// <param name="bytes">Message as Bytes</param>
        /// <returns>Hex encode</returns>
        private static string HexEncode(byte[] bytes)
        {
            StringBuilder hexEncode = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
                hexEncode.Append(bytes[i].ToString("X2"));
            return hexEncode.ToString().ToLower();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get gift cards from gyft api
        /// </summary>
        /// <returns>Gift cards</returns>
        async public Task<IList<GyftCardItem>> GetGiftCards()
        {
            var url = string.Format("{0}{1}?api_key={2}", _gyftApiBaseUrl, _shopCardUrl, _gyftApiKey);
            var response = await CallApi(url, WebRequestMethods.Http.Get);
            var giftCards = new List<GyftCardItem>();

            if (response != null)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                giftCards = JsonConvert.DeserializeObject<List<GyftCardItem>>(responseString);
            }

            return giftCards.Take(12).OrderBy(m=>m.Amount).ToList();
        }

        /// <summary>
        /// Purchase gift card
        /// </summary>
        /// <param name="giftPurchaseRequest">Gift purchase request</param>
        /// <returns>Gift card url</returns>
        async public Task<string> PurchaseGiftCard(GyftPurchaseRequest giftPurchaseRequest)
        {
            var jsonPostData = JsonConvert.SerializeObject(giftPurchaseRequest);
            var url = string.Format("{0}{1}?api_key={2}", _gyftApiBaseUrl, _purchaseCardUrl, _gyftApiKey);
            var response = await CallApi(url, WebRequestMethods.Http.Post, jsonPostData);
            if (response != null)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var deserializeResult = JsonConvert.DeserializeObject<dynamic>(responseString);
                if (deserializeResult != null)
                    return deserializeResult.url;
            }
            else
                throw new InvalidOperationException("Unable to complete the purchase. Please try again later.");

            return string.Empty;
        }

        #endregion
    }
}
