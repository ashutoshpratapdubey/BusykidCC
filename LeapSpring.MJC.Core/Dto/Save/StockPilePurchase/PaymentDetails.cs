using Newtonsoft.Json;

namespace LeapSpring.MJC.Core.Dto.Save.StockPilePurchase
{
    public class PaymentDetails
    {
        /// <summary>
        /// Gets or sets the payment type.
        /// </summary>
        [JsonProperty(PropertyName = "paymentType")]
        public string PaymentType { get; set; }
    }
}
