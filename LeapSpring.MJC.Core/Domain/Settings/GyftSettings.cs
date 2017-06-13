using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Domain.Settings
{
    /// <summary>
    /// Represents a gyft settings
    /// </summary>
    public class GyftSettings : BaseEntity
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the reseller reference
        /// </summary>
        public string ResellerReference { get; set; }
    }
}
