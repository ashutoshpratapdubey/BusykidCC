namespace LeapSpring.MJC.Core.Domain.Charities
{
    public class Charity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of charity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the charity logo url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the charity url
        /// </summary>
        public string CharityUrl { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
             
    }
}
