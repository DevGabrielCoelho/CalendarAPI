using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Dtos;
using CalendarAPI.Models;

namespace CalendarAPI.Mappers
{
    public static class ReminderMappers
    {
        public static Reminder RegisterReminder(CreateReminderDto dto, Event _event){
            Reminder reminder = new Reminder{
                CreatorEmail = _event.User.Email,
                Event = _event,
                EventId = _event.Id,
                Id = Guid.NewGuid().ToString(),
                MinutesBefore = dto.MinutesBefore
            };
            return reminder;
        }
    }
}