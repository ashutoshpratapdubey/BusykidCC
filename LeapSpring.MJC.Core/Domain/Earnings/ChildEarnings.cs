using LeapSpring.MJC.Core.Domain.Family;

namespace LeapSpring.MJC.Core.Domain.Earnings
{
    public class ChildEarnings : BaseEntity
    {
        /// <summary>
        /// Gets or sets the family member identifier
        /// </summary>
        public int FamilyMemberID { get; set; }

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

        /// <summary>
        /// Gets or sets the family member
        /// </summary>
        public virtual FamilyMember FamilyMember { get; set; }
    }
}
