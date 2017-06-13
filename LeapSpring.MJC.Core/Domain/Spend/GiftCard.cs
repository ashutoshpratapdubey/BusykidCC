using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Domain.Spend
{
    /// <summary>
    /// Represents a gift card
    /// </summary>
    public class GiftCard : BaseEntity
    {
        /// <summary>
        /// Gets or sets the card identifier
        /// </summary>
        public int CardId { get; set; }

        /// <summary>
        /// Gets or sets the merchant name
        /// </summary>
        public string GiftCardName { get; set; }

        /// <summary>
        /// Gets or sets the merchant identifier
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the merchant name
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the long description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the disclosure
        /// </summary>
        public string Disclosure { get; set; }

        /// <summary>
        /// Gets or sets the card currency code
        /// </summary>
        public string CardCurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the card value
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the merchant card template identifier
        /// </summary>
        public int MerchantCardTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the merchant icon image url
        /// </summary>
        public string MerchantIconImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the cover image url
        /// </summary>
        public string CoverImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the is featured
        /// </summary>
        public bool IsFeatured { get; set; }
    }
}
