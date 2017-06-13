using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Domain.Sms
{
    /// <summary>
    /// Represents a joke   
    /// </summary>
    public class Joke : BaseEntity
    {
        /// <summary>
        /// Gets or sets the joke text
        /// </summary>
        public string Text { get; set; }
    }
}
