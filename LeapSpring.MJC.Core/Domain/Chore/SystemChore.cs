using LeapSpring.MJC.Core.Enums;
using Newtonsoft.Json;

namespace LeapSpring.MJC.Core.Domain.Chore
{
    public class SystemChore : BaseEntity
    {
        /// <summary>
        /// Gets or sets core name. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the core value.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Gets ot sets the start age.
        /// </summary>
        public int StartAge { get; set; }

        /// <summary>
        /// Gets or sets the end age.
        /// </summary>
        public int EndAge { get; set; }

        /// <summary>
        /// Gets or sets the chore image url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the frequency type.
        /// </summary>
        public FrequencyType FrequencyType { get; set; }

        /// <summary>
        /// Gets or sets the child identifier
        /// </summary>
        [JsonIgnore]
        public int? ChildId { get; set; }

        /// <summary>
        /// Gets or sets the parent system chore identifier
        /// </summary>
        [JsonIgnore]
        public int? ParentSystemChoreId { get; set; }
    }
}