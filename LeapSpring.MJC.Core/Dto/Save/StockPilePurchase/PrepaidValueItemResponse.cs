using Newtonsoft.Json;
using System;

namespace LeapSpring.MJC.Core.Dto.Save.StockPilePurchase
{
    public class PrepaidValueItemResponse
    {
        /// <summary>
        /// Gets or sets the line item identifier.
        /// </summary>
        [JsonProperty(PropertyName = "lineItemID")]
        public Guid LineItemID { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }
    }
}
