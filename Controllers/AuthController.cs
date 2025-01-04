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
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext dbContext){
            _context = dbContext;
        }

        [HttpPost("register")]
        public IActionResult Register([FromForm]UserDto userDto){
            User user = new();
            user.Id = Guid.NewGuid().ToString();
            user.Email = userDto.Email;
            user.Name = userDto.Name;
            user.PassHash = "hashedpass";
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok();
        }
    }
}