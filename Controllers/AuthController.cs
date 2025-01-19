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
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _token;
        public AuthController(IPasswordHasher hasher, ITokenService token, IUserRepository userRepository){
            _hasher = hasher;
            _token = token;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm]UserDto userDto){
            userDto.Pass = _hasher.Hash(userDto.Pass);
            User user = UserMappers.RegisterUser(userDto);
            await _userRepository.AddUserAsync(user);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]LoginDto dto){
            User user = await _userRepository.GetUserByEmailAsync(dto.Email);
            bool Verify = _hasher.Verify(user.PassHash, dto.Pass);
            if(Verify){
                user.Token = _token.CreateToken(user);
                await _userRepository.UpdateTokenAsync(user.Id, user.Token);
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