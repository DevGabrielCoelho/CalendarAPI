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
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IPasswordHasher hasher,
            ITokenService tokenService,
            IUserRepository userRepository,
            IEmailVerificationService emailVerificationService,
            IEmailService emailService,
            ILogger<AuthController> logger
            )
        {
            _hasher = hasher;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _emailVerificationService = emailVerificationService;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserDto userDto)
        {
            _logger.LogInformation("Starting user registration for email: {Email}", userDto.Email);
            if (!_emailService.IsValidEmail(userDto.Email) || !_emailService.IsDomainValid(userDto.Email))
            {
                _logger.LogWarning("Invalid email or domain for email: {Email}", userDto.Email);
                return BadRequest("Invalid email or domain.");
            }
            try{
                if (await _userRepository.GetUserByEmailAsync(userDto.Email) != null)
                {
                    _logger.LogWarning("Email already registered: {Email}", userDto.Email);
                    return BadRequest("Email already registered.");
                }
            }catch(Exception){}
            userDto.Pass = _hasher.Hash(userDto.Pass);
            var user = UserMappers.RegisterUser(userDto);
            await _userRepository.AddUserAsync(user);
            await SendValidationCode(userDto.Email);

            _logger.LogInformation("User registered successfully for email: {Email}", userDto.Email);

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
            try
            {
                if (!_emailService.IsValidEmail(dto.Email) || !_emailService.IsDomainValid(dto.Email))
                    return BadRequest("Invalid email or domain.");

                var user = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (user == null || !_hasher.Verify(user.PassHash, dto.Pass))
                {
                    _logger.LogWarning("Invalid login attempt for email: {Email}", dto.Email);
                    return BadRequest("Invalid email or password.");
                }

                user.Token = _tokenService.CreateToken(user);
                await _userRepository.UpdateTokenAsync(user.Id, user.Token);
                _logger.LogInformation("User successfully logged in for email: {Email}", dto.Email);
                return Ok(new { Token = user.Token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing login for email: {Email}", dto.Email);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
