using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.Core.Domain.Subscription
{
    public class TransactionStatusFileDetails : BaseEntity
    {
        public string FileName { get; set; }
        public DateTime FileUsedDate { get; set; }
        public bool ActiveStatus { get; set; }
    }

    public class EventStatusLog : BaseEntity
    {
        public int TransactionID  { get; set; }
        public string EventTypeStatus { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
