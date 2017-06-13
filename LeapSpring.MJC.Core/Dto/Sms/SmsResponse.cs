using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Dto.Sms
{
    /// <summary>
    /// Represents a twilio sms response
    /// </summary>
    public class SmsResponse
    {
        /// <summary>
        /// Gets or sets the ToCountry
        /// </summary>
        public string ToCountry { get; set; }

        /// <summary>
        /// Gets or sets the ToState
        /// </summary>
        public string ToState { get; set; }

        /// <summary>
        /// Gets or sets the SmsMessageSid
        /// </summary>
        public string SmsMessageSid { get; set; }

        /// <summary>
        /// Gets or sets the NumMedia
        /// </summary>
        public string NumMedia { get; set; }

        /// <summary>
        /// Gets or sets the ToCity
        /// </summary>
        public string ToCity { get; set; }

        /// <summary>
        /// Gets or sets the FromZip
        /// </summary>
        public string FromZip { get; set; }

        /// <summary>
        /// Gets or sets the SmsSid
        /// </summary>
        public string SmsSid { get; set; }

        /// <summary>
        /// Gets or sets the FromState
        /// </summary>
        public string FromState { get; set; }

        /// <summary>
        /// Gets or sets the SmsStatus
        /// </summary>
        public string SmsStatus { get; set; }

        /// <summary>
        /// Gets or sets the FromCity
        /// </summary>
        public string FromCity { get; set; }

        /// <summary>
        /// Gets or sets the Body message
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the FromCountry
        /// </summary>
        public string FromCountry { get; set; }

        /// <summary>
        /// Gets or sets the To
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the ToZip
        /// </summary>
        public string ToZip { get; set; }

        /// <summary>
        /// Gets or sets the NumSegments
        /// </summary>
        public string NumSegments { get; set; }

        /// <summary>
        /// Gets or sets the MessageSid
        /// </summary>
        public string MessageSid { get; set; }

        /// <summary>
        /// Gets or sets the AccountSid
        /// </summary>
        public string AccountSid { get; set; }

        /// <summary>
        /// Gets or sets the From
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the Api Version
        /// </summary>
        public string ApiVersion { get; set; }
    }
}
