using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LeapSpring.MJC.Core.Dto.Save.StockPilePurchase
{
    public class PurchaseStockResponse
    {
        /// <summary>
        /// Gets or sets the preamble.
        /// </summary>
        [JsonProperty(PropertyName = "preamble")]
        public Preamble Preamble { get; set; }

        /// <summary>
        /// Gets or sets the status of the request.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error details.
        /// </summary>
        [JsonProperty(PropertyName = "errorDetail")]
        public string ErrorDetail { get; set; }

        /// <summary>
        /// Gets or sets the paymeny response.
        /// </summary>
        [JsonProperty(PropertyName = "paymentResponse")]
        public string PaymentResponse { get; set; }

        /// <summary>
        /// Gets or sets the prepaid value item response.
        /// </summary>
        [JsonProperty(PropertyName = "prepaidValueItemResponses")]
        public List<PrepaidValueItemResponse> PrepaidValueItemResponse { get; set; }
    }
}
