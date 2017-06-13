using LeapSpring.MJC.Core.Enums;

namespace LeapSpring.MJC.Core.Dto.Chores
{
    public class SuggestedChore
    {
        /// <summary>
        /// Gets or sets the system chore identifier.
        /// </summary>
        public int? SystemChoreId { get; set; }

        /// <summary>
        /// Gets or sets the name of chore.
        /// </summary>
        public string NameofChore { get; set; }

        /// <summary>
        /// Gets or sets the value of the chore.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the chore image url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the default frequency type.
        /// </summary>
        public FrequencyType FrequencyType { get; set; }
    }
}
