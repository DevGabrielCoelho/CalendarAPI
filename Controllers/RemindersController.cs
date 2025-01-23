using CalendarAPI.Dtos;
using CalendarAPI.Interfaces;
using CalendarAPI.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/reminders")]
    public class RemindersController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IReminderRepository _reminderRepository;

        public RemindersController(IEventRepository eventRepository, IReminderRepository reminderRepository)
        {
            _eventRepository = eventRepository;
            _reminderRepository = reminderRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReminder([FromForm] CreateReminderDto dto)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(dto.EventId);
            if (eventEntity == null)
            {
                return NotFound("Event not found.");
            }

            var reminder = ReminderMappers.RegisterReminder(dto, eventEntity);

            eventEntity.Reminders.Add(reminder);
            await _reminderRepository.AddAsync(reminder);
            await _eventRepository.UpdateAsync(eventEntity);

            return Ok("Reminder created successfully.");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder([FromRoute] string id)
        {
            var reminder = await _reminderRepository.GetByIdAsync(id);
            if (reminder == null)
            {
                return NotFound("Reminder not found.");
            }

            var eventEntity = reminder.Event;
            eventEntity.Reminders.Remove(reminder);
            await _eventRepository.UpdateAsync(eventEntity);
            await _reminderRepository.RemoveAsync(reminder);

            return Ok("Reminder removed successfully.");
        }
    }
}
