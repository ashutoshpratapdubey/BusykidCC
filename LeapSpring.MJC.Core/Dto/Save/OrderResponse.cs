using Newtonsoft.Json;
using System.Collections.Generic;

namespace LeapSpring.MJC.Core.Dto.Save
{
    public class OrderResponse
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "orders")]
        public List<Order> Orders { get; set; }
    }
}
