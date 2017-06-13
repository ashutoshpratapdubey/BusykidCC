using Newtonsoft.Json;

namespace LeapSpring.MJC.Core.Dto.Save.StockPilePurchase
{
    public class Purchaser
    {
        /// <summary>
        /// Gets or sets the email of the purchaser.
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name of the purchaser.
        /// </summary>
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the purchaser.
        /// </summary>
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
    }
}
