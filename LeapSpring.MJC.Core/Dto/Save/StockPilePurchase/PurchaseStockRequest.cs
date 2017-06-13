using Newtonsoft.Json;
using System.Collections.Generic;

namespace LeapSpring.MJC.Core.Dto.Save.StockPilePurchase
{
    public class PurchaseStockRequest
    {
        /// <summary>
        /// Gets or sets the preamble.
        /// </summary>
        [JsonProperty(PropertyName = "preamble")]
        public Preamble Preamble { get; set; }

        /// <summary>
        /// Gets or sets the purchaser.
        /// </summary>
        [JsonProperty(PropertyName = "purchaser")]
        public Purchaser Purchaser { get; set; }

        /// <summary>
        /// Gets or sets the payment details.
        /// </summary>
        [JsonProperty(PropertyName = "paymentDetails")]
        public PaymentDetails PaymentDetails { get; set; }

        /// <summary>
        /// Gets or sets the prepaid value item requests.
        /// </summary>
        [JsonProperty(PropertyName = "prepaidValueItemRequests")]
        public List<PrepaidValueItemRequest> PrepaidValueItemRequests { get; set; }
    }
}
