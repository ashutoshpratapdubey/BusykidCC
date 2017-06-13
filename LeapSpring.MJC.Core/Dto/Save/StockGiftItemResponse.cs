using Newtonsoft.Json;
using System.Collections.Generic;

namespace LeapSpring.MJC.Core.Dto.Save
{
    public class StockGiftItemResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("stockItems")]
        public List<StockGiftItem> StockItems { get; set; }
    }
}
