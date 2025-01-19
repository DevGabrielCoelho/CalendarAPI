using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Data;
using CalendarAPI.Dtos;
using CalendarAPI.Interfaces;
using CalendarAPI.Models;
using CalendarAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;

        public EventsController(IUserRepository userRepository, IEventRepository eventRepository)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateEventDto dto, [FromForm] string email, [FromForm] string token, [FromForm] List<string> emails)
        {
            System.Console.WriteLine(dto.Title);
            User user = await _userRepository.GetUserByEmailAsync(email);
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
                    User = user,
                    GuestsEmails = emails
                    
                };
                user.Events.Add(_event);
                await _eventRepository.AddEventAsync(_event);
                await _userRepository.UpdateUserAsync(user);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListEvents()
        {
            return Ok(await _eventRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ListEventsById([FromRoute] string id)
        {
            return Ok(await _eventRepository.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AttEventsById([FromRoute] string id, [FromForm] CreateEventDto dto)
        {
            Event _event = await _eventRepository.GetByIdAsync(id);
            _event.DateEnd = dto.DateEnd;
            _event.DateStart = dto.DateStart;
            _event.Description = dto.Description;
            _event.Location = dto.Location;
            _event.Title = dto.Title;

            await _eventRepository.UpdateAsync(_event);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DelEventById([FromRoute] string id)
        {
            _eventRepository.RemoveByIdAsync(id);
            return Ok();
        }


    }
}