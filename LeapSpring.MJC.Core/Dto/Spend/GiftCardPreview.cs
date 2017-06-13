using LeapSpring.MJC.Core.Domain.Spend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Dto.Spend
{
    /// <summary>
    /// Represents a gift card preview
    /// </summary>
    public class GiftCardPreview
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the disclosure
        /// </summary>
        public string Disclosure { get; set; }

        /// <summary>
        /// Gets or sets the cover image url
        /// </summary>
        public string CoverImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the gift cards
        /// </summary>
        public IList<GiftCard> GiftCards { get; set; }

        /// <summary>
        /// Gets or sets the is featured
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// Gets or sets the minimum amount
        /// </summary>
        public decimal MinAmount { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount
        /// </summary>
        public decimal MaxAmount { get; set; }
    }
}
