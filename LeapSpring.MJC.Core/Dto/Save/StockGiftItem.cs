using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LeapSpring.MJC.Core.Dto.Save
{
    public class StockGiftItem
    {
        /// <summary>
        /// Gets or sets the item code.
        /// </summary>
        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets the stock symbol.
        /// </summary>
        [JsonProperty("stockSymbol")]
        public string StockSymbol { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [JsonProperty("companyName")]
        public string CompnyName { get; set; }

        /// <summary>
        /// Gets or sets the company popular name.
        /// </summary>
        [JsonProperty("companyPopularName")]
        public string CompanyPopularName { get; set; }

        /// <summary>
        /// Gets or sets the brand name.
        /// </summary>
        [JsonProperty("brandName")]
        public string BrandName { get; set; }

        /// <summary>
        /// Gets or sets the company relation name.
        /// </summary>
        [JsonProperty("companyRelation")]
        public string CompanyRelationName { get; set; }

        /// <summary>
        /// Gets or sets the logo url.
        /// </summary>
        [JsonProperty("logoURL")]
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the enable.
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the generic.
        /// </summary>
        [JsonProperty("generic")]
        public bool Generic { get; set; }

        /// <summary>
        /// Gets or sets the stock item categories.
        /// </summary>
        [JsonProperty("categories")]
        public List<string> Categories { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the last updated.
        /// </summary>
        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the quote of the stock
        /// </summary>
        public GiftStockQuote Quote { get; set; }

        /// <summary>
        /// Gets or sets is stock or not.
        /// </summary>
        public bool IsStock { get; set; }

        /// <summary>
        /// Gets or sets the amount to purchase stock.
        /// </summary>
        public decimal Amount { get; set; }
    }
}
