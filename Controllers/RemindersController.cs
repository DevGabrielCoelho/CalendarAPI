using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Data;
using CalendarAPI.Dtos;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/reminders")]
    public class RemindersController : ControllerBase
    {
        public readonly AppDbContext _context;
        public RemindersController(AppDbContext context){
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateReminders([FromForm] CreateReminderDto dto){
            Event _event = _context.Events
                                .Include(x => x.User)
                                .Include(x => x.Reminders)
                                .FirstOrDefault(x => x.Id == dto.EventId);
            System.Console.WriteLine(_event.Id);
            System.Console.WriteLine(_event.User.Id);
            Reminder reminder = new Reminder{
                CreatorEmail = _event.User.Email,
                Event = _event,
                EventId = _event.Id,
                Id = Guid.NewGuid().ToString(),
                TimeBefore = dto.TimeBefore
            };
            _event.Reminders.Add(reminder);
            _context.Reminders.Add(reminder);
            _context.Events.Update(_event);
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReminders([FromRoute] string id){
            Reminder reminder = _context.Reminders.Include(x => x.Event).FirstOrDefault(x => x.Id == id);
            Event _event = reminder.Event;
            _event.Reminders.Remove(reminder);
            _context.Events.Update(_event);
            _context.Reminders.Remove(reminder);
            _context.SaveChanges();
            return Ok();
        }
    }
}