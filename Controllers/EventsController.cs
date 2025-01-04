using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Data;
using CalendarAPI.Dtos;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context){
            _context = context;
        }

        [HttpPost]
        public IActionResult Criate([FromForm] CreateEventDto dto, string email, string token){
            User user = _context.Users.FirstOrDefault(x => x.Email == email);
            Event _event;
            if(user.Token == token){
               _event = new Event{
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

    }
}