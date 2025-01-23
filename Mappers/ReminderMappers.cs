using CalendarAPI.Dtos;
using CalendarAPI.Models;

namespace CalendarAPI.Mappers
{
    public static class ReminderMappers
    {
        public static Reminder RegisterReminder(CreateReminderDto dto, Event _event)
        {
            return new Reminder
            {
                Id = Guid.NewGuid().ToString(),
                CreatorEmail = _event.User.Email,
                Event = _event,
                EventId = _event.Id,
                MinutesBefore = dto.MinutesBefore
            };
        }
    }
}
