using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Data;
using CalendarAPI.Dtos;
using CalendarAPI.Interfaces;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/reminders")]
    public class RemindersController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IReminderRepository _reminderRepository;
        public RemindersController(IEventRepository eventRepository, IReminderRepository reminderRepository){
            _eventRepository = eventRepository;
            _reminderRepository = reminderRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReminders([FromForm] CreateReminderDto dto){
            Event _event = await _eventRepository.GetByIdAsync(dto.EventId);
            Reminder reminder = new Reminder{
                CreatorEmail = _event.User.Email,
                Event = _event,
                EventId = _event.Id,
                Id = Guid.NewGuid().ToString(),
                MinutesBefore = dto.MinutesBefore
            };
            _event.Reminders.Add(reminder);
            await _reminderRepository.AddAsync(reminder);
            await _eventRepository.UpdateAsync(_event);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminders([FromRoute] string id){
            Reminder reminder = await _reminderRepository.GetByIdAsync(id);
            Event _event = reminder.Event;
            _event.Reminders.Remove(reminder);
            await _eventRepository.UpdateAsync(_event);
            await _reminderRepository.RemoveAsync(reminder);
            return Ok();
        }
    }
}