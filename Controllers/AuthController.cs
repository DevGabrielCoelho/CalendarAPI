using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarAPI.Data;
using CalendarAPI.Dtos;
using CalendarAPI.Interfaces;
using CalendarAPI.Mappers;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _token;
        public AuthController(AppDbContext dbContext, IPasswordHasher hasher, ITokenService token){
            _context = dbContext;
            _hasher = hasher;
            _token = token;
        }

        [HttpPost("register")]
        public IActionResult Register([FromForm]UserDto userDto){
            userDto.Pass = _hasher.Hash(userDto.Pass);
            User user = UserMappers.RegisterUser(userDto);
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromForm]LoginDto dto){
            User user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            bool Verify = _hasher.Verify(user.PassHash, dto.Pass);
            if(Verify){
                user.Token = _token.CreateToken(user);
                _context.Users.Where(x => x.Id == user.Id).ExecuteUpdate(x => x
                .SetProperty(x => x.Token, user.Token));
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test(){
            return Ok();
        }
    }
}