using Newtonsoft.Json;
using System;

namespace LeapSpring.MJC.Core.Dto.Save.StockPilePurchase
{
    public class PrepaidValueItemRequest
    {
        /// <summary>
        /// Gets or sets the line item identifer.
        /// </summary>
        [JsonProperty(PropertyName = "lineItemID")]
        public Guid LineItemID { get; set; }

        /// <summary>
        /// Gets or sets the value of stock to be purchased. 
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [JsonProperty(PropertyName = "currencyCode")]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the item type.
        /// </summary>
        [JsonProperty(PropertyName = "itemType")]
        public string ItemType { get; set; }

        /// <summary>
        /// Gets or sets the item code.
        /// </summary>
        [JsonProperty(PropertyName = "itemCode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets the delivery option.
        /// </summary>
        [JsonProperty(PropertyName = "deliveryOption")]
        public string DeliveryOption { get; set; }

        /// <summary>
        /// Gets or sets the is gift.
        /// </summary>
        [JsonProperty(PropertyName = "isGift")]
        public bool IsGift { get; set; }

        /// <summary>
        /// Gets or sets the recipient.
        /// </summary>
        [JsonProperty(PropertyName = "recipient")]
        public Purchaser Recipient { get; set; }
    }
}
