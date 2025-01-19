using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarAPI.Dtos
{
    public class CreateReminderDto
    {
        public string EventId { get; set; } = string.Empty;
        public double MinutesBefore { get; set; }
    }
}