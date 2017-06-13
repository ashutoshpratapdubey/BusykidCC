using Newtonsoft.Json;
using System.Collections.Generic;

namespace LeapSpring.MJC.Core.Dto.Save
{
    public class StockGiftQuoteResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("giftStockQuotes")]
        public List<GiftStockQuote> StockGiftQuotes { get; set; }
    }
}
