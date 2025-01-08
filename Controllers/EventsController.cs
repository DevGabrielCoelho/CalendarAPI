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
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Criate([FromForm] CreateEventDto dto, string email, string token)
        {
            User user = _context.Users.FirstOrDefault(x => x.Email == email);
            Event _event;
            if (user.Token == token)
            {
                _event = new Event
                {
                    DateStart = dto.DateStart,
                    DateEnd = dto.DateEnd,
                    Description = dto.Description,
                    Id = Guid.NewGuid().ToString(),
                    Location = dto.Location,
                    Title = dto.Title,
                    UserId = user.Id,
                    User = user
                };
                user.Events.Add(_event);
                _context.Events.Add(_event);
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult ListEvents()
        {
            return Ok(_context.Events.Include(x => x.User).Include(x => x.Reminders).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult ListEventsById([FromRoute] string id){
            return Ok(_context.Events.Include(x => x.User).FirstOrDefault(x => x.Id == id));
        }

        [HttpPut("{id}")]
        public IActionResult AttEventsById([FromRoute] string id, [FromForm] CreateEventDto dto){
            Event _event = _context.Events.FirstOrDefault(x => x.Id == id);
            _event.DateEnd = dto.DateEnd;
            _event.DateStart = dto.DateStart;
            _event.Description = dto.Description;
            _event.Location = dto.Location;
            _event.Title = dto.Title;

            _context.Events.Update(_event);

            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DelEventById([FromRoute] string id){
            _context.Events.Remove(_context.Events.FirstOrDefault(x => x.Id == id));
            _context.SaveChanges();
            return Ok();
        }


    }
}