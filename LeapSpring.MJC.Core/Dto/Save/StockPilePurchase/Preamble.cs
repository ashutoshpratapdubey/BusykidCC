using Newtonsoft.Json;
using System;

namespace LeapSpring.MJC.Core.Dto.Save.StockPilePurchase
{
    public class Preamble
    {
        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        [JsonProperty(PropertyName = "transactionID")]
        public Guid TransactionID { get; set; }

        /// <summary>
        /// Gets or sets the institution identifier.
        /// </summary>
        [JsonProperty(PropertyName = "institutionID")]
        public string InstitutionID { get; set; }

        /// <summary>
        /// Gets or sets the partner program.
        /// </summary>
        [JsonProperty(PropertyName = "partnerProgram")]
        public string PartnerProgram { get; set; }
    }
}
