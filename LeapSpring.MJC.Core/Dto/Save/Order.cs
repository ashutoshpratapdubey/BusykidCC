using Newtonsoft.Json;

namespace LeapSpring.MJC.Core.Dto.Save
{
    public class Order
    {
        /// <summary>
        /// Gets or sets the keycode.
        /// </summary>
        [JsonProperty(PropertyName = "keyCode")]
        public string KeyCode { get; set; }

        /// <summary>
        /// Gets or sets the redeem url of the stock purchased.
        /// </summary>
        [JsonProperty(PropertyName = "redeemUrl")]
        public string RedeemUrl { get; set; }
    }
}
