using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeapSpring.MJC.Core.Domain.StatusLog
{
    public class Statuslog : BaseEntity
    {
        public int ChildFamilyMemberID { get; set; }

        public int ParentFamilyMemberID { get; set; }
        public decimal Amount { get; set; }
        public DateTime? currentdate { get; set; }        
        public int Status { get; set; }

    }
}
