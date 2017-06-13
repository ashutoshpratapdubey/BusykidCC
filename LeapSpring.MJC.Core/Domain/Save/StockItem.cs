using System;

namespace LeapSpring.MJC.Core.Domain.Save
{
    public class StockItem : BaseEntity
    {
        /// <summary>
        /// Gets or sets the item code.
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets the stock symbol.
        /// </summary>
        public string StockSymbol { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the company popular name.
        /// </summary>
        public string CompanyPopularName { get; set; }

        /// <summary>
        /// Gets or sets the brand name.
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// Gets or sets the company relation name.
        /// </summary>
        public string CompanyRelationName { get; set; }

        /// <summary>
        /// Gets or sets the logo url.
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the enable.
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the generic.
        /// </summary>
        public bool Generic { get; set; }

        /// <summary>       
        /// Gets or sets the date created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the stock price.
        /// </summary>
        public decimal StockPrice { get; set; }

        /// <summary>
        /// Gets or sets the retrieved date of stock price.
        /// </summary>
        public DateTime StockPriceRetrievedAt { get; set; }

        /// <summary>
        /// Gets or sets the is is featured stock.
        /// </summary>
        public bool IsFeaturedStock { get; set; }
    }
}
