using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Exceptions;
using NewsAggrigation.BLL.Services.Auth;
using NewsAggrigation.BLL.Services.Helper;

namespace NewsAggrigation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request)
        {
            try
            {
                _logger.LogInformation("Registering user with email: {Email}", request.Email);
                var result = await _authService.RegisterAsync(request);
                _logger.LogInformation("User registered successfully: {Email}", request.Email);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error during registration for email: {Email}", request.Email);
                return BadRequest(new { ex.Message });
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict error during registration for email: {Email}", request.Email);
                return Conflict(new { ex.Message });
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception during registration for email: {Email}", request.Email);
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for email: {Email}", request.Email);
                return StatusCode(500, new { Message = "Registration failed. " + ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Attempting login for username: {Username}", request.Username);
                var result = await _authService.LoginAsync(request);
                _logger.LogInformation("User logged in successfully: {Username}", request.Username);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error during login for username: {Username}", request.Username);
                return BadRequest(new { ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex, "Unauthorized login attempt for username: {Username}", request.Username);
                return Unauthorized();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found during login for username: {Username}", request.Username);
                return NotFound(new { ex.Message });
            }
            catch (ApiExceptionHelper ex)
            {
                _logger.LogWarning(ex, "API exception during login for username: {Username}", request.Username);
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for username: {Username}", request.Username);
                return StatusCode(500, new { Message = "Login failed. " + ex.Message });
            }
        }
    }
}
