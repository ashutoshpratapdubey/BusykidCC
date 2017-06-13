using Newtonsoft.Json;
using System;

namespace LeapSpring.MJC.Core.Dto.Save
{
    public class GiftStockQuote
    {
        /// <summary>
        /// Gets or sets the stock symbol.
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the closing price.
        /// </summary>
        [JsonProperty("closingPrice")]
        public decimal ClosingPrice { get; set; }

        /// <summary>
        /// Gets or sets the date retrieved at.
        /// </summary>
        [JsonProperty("retrievedAt")]
        public DateTime RetrievedAt { get; set; }
    }
}
