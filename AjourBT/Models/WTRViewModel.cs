using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourBT.Models
{
    public class FactorData
    {
        public CalendarItemType Factor {get; set;}
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public byte Hours { get; set; }
        public int WeekNumber { get; set; }
        public string Location { get; set; }
    }

    public class WTRViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ID { get; set; }
        public List<FactorData> FactorDetails { get; set; }
    }
}