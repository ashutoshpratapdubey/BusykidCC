using LeapSpring.MJC.Core.Domain.Chore;
using System.Collections.Generic;

namespace LeapSpring.MJC.Core.Dto.Chores
{
    public class SuggestedChores
    {
        /// <summary>
        /// Gets or sets the system chores
        /// </summary>
        public List<SystemChore> SystemChores { get; set; }

        /// <summary>
        /// Gets or sets the total count
        /// </summary>
        public int TotalCount { get; set; }
    }
}
