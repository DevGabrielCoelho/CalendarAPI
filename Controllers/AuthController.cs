using CalendarAPI.Dtos;
using CalendarAPI.Interfaces;
using CalendarAPI.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _tokenService;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IEmailService _emailService;

        public AuthController(
            IPasswordHasher hasher,
            ITokenService tokenService,
            IUserRepository userRepository,
            IEmailVerificationService emailVerificationService,
            IEmailService emailService)
        {
            _hasher = hasher;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _emailVerificationService = emailVerificationService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserDto userDto)
        {
            if (!_emailService.IsValidEmail(userDto.Email) || !_emailService.IsDomainValid(userDto.Email))
                return BadRequest("Invalid email or domain.");
            if (await _userRepository.GetUserByEmailAsync(userDto.Email) != null)
                return BadRequest("Email already registered.");
            userDto.Pass = _hasher.Hash(userDto.Pass);
            var user = UserMappers.RegisterUser(userDto);
            await _userRepository.AddUserAsync(user);
            await SendValidationCode(userDto.Email);
            return Ok("User registered successfully.");
        }

        [HttpPost("send-code")]
        public async Task<IActionResult> SendValidationCode([FromQuery] string email)
        {
            if (!_emailService.IsValidEmail(email) || !_emailService.IsDomainValid(email))
                return BadRequest("Invalid email or domain.");

            if (await _userRepository.GetUserByEmailAsync(email) == null)
                return NotFound("User not found.");

            var code = _emailVerificationService.GenerateCode(email);
            await _emailService.SendEmailAsync(email, "Code Validator", $"Your verification code is: {code}");
            return Ok("Verification code sent.");
        }

        [HttpPost("validate-code")]
        public async Task<IActionResult> ValidateCode([FromQuery] string email, [FromQuery] string code)
        {
            if (!_emailService.IsValidEmail(email) || !_emailService.IsDomainValid(email))
                return BadRequest("Invalid email or domain.");

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound("User not found.");
            
            if (!_emailVerificationService.ValidateVerificationCode(email, code))
                return BadRequest("Invalid verification code.");
            
            user.Validated = true;
            await _userRepository.UpdateUserAsync(user);
            return Ok("Code validated successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
        {
            if (!_emailService.IsValidEmail(dto.Email) || !_emailService.IsDomainValid(dto.Email))
                return BadRequest("Invalid email or domain.");

            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null || !_hasher.Verify(user.PassHash, dto.Pass))
                return BadRequest("Invalid email or password.");
            
            user.Token = _tokenService.CreateToken(user);
            await _userRepository.UpdateTokenAsync(user.Id, user.Token);
            return Ok(new { Token = user.Token });
        }
    }
}
