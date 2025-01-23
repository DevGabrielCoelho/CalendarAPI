using CalendarAPI.Dtos;
using CalendarAPI.Interfaces;
using CalendarAPI.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEmailService _emailService;

        public EventsController(IUserRepository userRepository, IEventRepository eventRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _emailService = emailService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateEventDto dto, [FromForm] string email, [FromForm] string token, [FromForm] List<string> emails)
        {
            if (!_emailService.IsValidEmail(email) || !_emailService.IsDomainValid(email))
                return BadRequest("Invalid email or domain.");
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (string.IsNullOrEmpty(user?.Token)) 
                return BadRequest("Login error, try logging in again");
            
            if (!user.Validated) 
                return BadRequest("Your email is not validated");

            if (user.Token != token)
                return BadRequest("Invalid token");

            var newEvent = EventMappers.RegisterEvent(dto, emails, user);
            user.Events.Add(newEvent);

            await _eventRepository.AddEventAsync(newEvent);
            await _userRepository.UpdateUserAsync(user);
            
            return Ok("Event created successfully");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListEvents()
        {
            var events = await _eventRepository.GetAllAsync();
            return Ok(events);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById([FromRoute] string id)
        {
            var eventItem = await _eventRepository.GetByIdAsync(id);
            if (eventItem == null)
                return NotFound("Event not found");
            
            return Ok(eventItem);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent([FromRoute] string id, [FromForm] CreateEventDto dto)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(id);
            if (existingEvent == null)
                return NotFound("Event not found");
            
            existingEvent = EventMappers.EditEvent(dto, existingEvent);
            await _eventRepository.UpdateAsync(existingEvent);
            
            return Ok("Event updated successfully");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent([FromRoute] string id)
        {
            var eventToDelete = await _eventRepository.GetByIdAsync(id);
            if (eventToDelete == null)
                return NotFound("Event not found");

            await _eventRepository.RemoveByIdAsync(id);
            return Ok("Event deleted successfully");
        }
    }
}
