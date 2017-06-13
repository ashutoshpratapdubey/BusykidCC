using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Dto.Spend
{
    /// <summary>
    /// Represents a gift purchase request
    /// </summary>
    public class GyftPurchaseRequest
    {
        /// <summary>
        /// Gets or sets the shop card identifier
        /// </summary>
        [JsonProperty("shop_card_id")]
        public int ShopCardId { get; set; }

        /// <summary>
        /// Gets or sets the to_email
        /// </summary>
        [JsonProperty("to_email")]
        public string ToEmail { get; set; }

        /// <summary>
        /// Gets or sets the reseller reference code
        /// </summary>
        [JsonProperty("reseller_reference")]
        public string ResellerReference { get; set; }

        /// <summary>
        /// Gets or sets the notes
        /// </summary>
        [JsonProperty("notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the gender
        /// </summary>
        [JsonProperty("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the birthday
        /// </summary>
        [JsonProperty("birthday")]
        public string Birthday { get; set; }
    }
}
