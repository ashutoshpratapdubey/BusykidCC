using System.ComponentModel.DataAnnotations.Schema;

namespace LeapSpring.MJC.Core.Domain
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Get or set the identifier
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
