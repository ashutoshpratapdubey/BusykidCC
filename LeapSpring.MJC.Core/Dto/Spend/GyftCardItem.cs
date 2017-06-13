using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Dto.Spend
{
    /// <summary>
    /// Represents a gyft api response gift cards
    /// </summary>
    public class GyftCardItem
    {
        /// <summary>
        /// Gets or sets the gift card identifier
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the merchant identifier
        /// </summary>
        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the merchant name
        /// </summary>
        [JsonProperty("merchant_name")]
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the merchant name
        /// </summary>
        [JsonProperty("gift_card_name")]
        public string GiftCardName { get; set; }

        /// <summary>
        /// Gets or sets the long description
        /// </summary>
        [JsonProperty("long_description")]
        public string LongDescription { get; set; }

        /// <summary>
        /// Gets or sets the card currency code
        /// </summary>
        [JsonProperty("card_currency_code")]
        public string CardCurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the card value
        /// </summary>
        [JsonProperty("opening_balance")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the merchant card template identifier
        /// </summary>
        [JsonProperty("merchant_card_template_id")]
        public int MerchantCardTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the merchant icon image url
        /// </summary>
        [JsonProperty("merchant_icon_image_url_hd")]
        public string MerchantIconImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the cover image url
        /// </summary>
        [JsonProperty("cover_image_url_hd")]
        public string CoverImageUrl { get; set; }
    }
}
