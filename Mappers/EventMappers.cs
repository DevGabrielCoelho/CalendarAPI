using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Dtos;
using CalendarAPI.Models;

namespace CalendarAPI.Mappers
{
    public static class EventMappers
    {
        public static Event RegisterEvent(CreateEventDto dto, List<string> emails, User user){
            Event @event = new Event
                {
                    DateStart = dto.DateStart,
                    DateEnd = dto.DateEnd,
                    Description = dto.Description,
                    Id = Guid.NewGuid().ToString(),
                    Location = dto.Location,
                    Title = dto.Title,
                    UserId = user.Id,
                    User = user,
                    GuestsEmails = emails
                    
                };
            return @event;
        }

        public static Event EditEvent(CreateEventDto dto, Event _event){
            _event.DateEnd = dto.DateEnd;
            _event.DateStart = dto.DateStart;
            _event.Description = dto.Description;
            _event.Location = dto.Location;
            _event.Title = dto.Title;
            return _event;
        }
    }
}