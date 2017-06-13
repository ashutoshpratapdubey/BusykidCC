using LeapSpring.MJC.Core.Enums;
using Newtonsoft.Json;
using System;

namespace LeapSpring.MJC.Core.Dto.Banking
{
    public class MessageEvent
    {
        [JsonProperty("UserEventId")]
        public int UserEventId { get; set; }

        [JsonProperty("UserEventTypeId")]
        public MessageEventEnum UserEventType { get; set; }

        [JsonProperty("ImpactedCustomerId")]
        public int CustomerId { get; set; }

        [JsonProperty("TransactionId")]
        public int TransactionId { get; set; }

        [JsonProperty("TransactionStatus")]
        public string TransactionStatus { get; set; }

        [JsonProperty("TransactionReturnCode")]
        public string TransactionReturnCode { get; set; }

        [JsonProperty("EventTimeUtc")]
        public DateTime EventTime { get; set; }
    }
}
