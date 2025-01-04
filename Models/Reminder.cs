using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarAPI.Models
{
    public class Reminder
    {
        public string Id { get; set; }
        public string EventId { get; set; }
        public Event Event { get; set; }
        public string TimeBefore { get; set; }
        public string CreatorEmail { get; set; }
    }
}