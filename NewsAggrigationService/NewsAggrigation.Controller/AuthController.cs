using Microsoft.AspNetCore.Mvc;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.BLL.Services.Auth;
using NewsAggrigation.BLL.Services.Helper;

namespace NewsAggrigation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(result);
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Registration failed." + ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                if (result == null)
                    return StatusCode(401, new { Message = "Invalid email or password." });

                return Ok(result);
            }
            catch (ApiExceptionHelper ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Login failed." + ex.Message });
            }
        }
    }
}
