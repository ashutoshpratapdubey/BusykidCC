using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Domain.Settings
{
    /// <summary>
    /// Represents a default allocation by age
    /// </summary>
    public class AllocationByAge : BaseEntity
    {
        /// <summary>
        /// Gets or sets the age
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the save
        /// </summary>
        public decimal Save { get; set; }

        /// <summary>
        /// Gets or sets the share
        /// </summary>
        public decimal Share { get; set; }

        /// <summary>
        /// Gets or sets the spend
        /// </summary>
        public decimal Spend { get; set; }

    }
}
