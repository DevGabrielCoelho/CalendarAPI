using CalendarAPI.Dtos;
using CalendarAPI.Models;

namespace CalendarAPI.Mappers
{
    public static class EventMappers
    {
        public static Event RegisterEvent(CreateEventDto dto, List<string> emails, User user)
        {
            return new Event
            {
                Id = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Description = dto.Description,
                DateStart = dto.DateStart,
                DateEnd = dto.DateEnd,
                Location = dto.Location,
                UserId = user.Id,
                User = user,
                GuestsEmails = emails
            };
        }

        public static Event EditEvent(CreateEventDto dto, Event _event)
        {
            _event.DateStart = dto.DateStart;
            _event.DateEnd = dto.DateEnd;
            _event.Description = dto.Description;
            _event.Location = dto.Location;
            _event.Title = dto.Title;
            return _event;
        }
    }
}
