using Microsoft.Extensions.Logging;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.BLL.Exceptions;
using NewsAggrigation.BLL.Services.Helper;
using NewsAggrigation.DAL.Models;
using NewsAggrigation.DAL.Repositories.Auth;
using System;
using System.Threading.Tasks;

namespace NewsAggrigation.BLL.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly TokenService _tokenService;
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, TokenService tokenService, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest dto)
        {
            try
            {
                var user = await _authRepository.GetUserByUsernameAsync(dto.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Unauthorized login attempt for user: {Username}", dto.Username);
                    throw new UnauthorizedException("Invalid username or password.");
                }

                var token = _tokenService.CreateToken(user.UserId, user.Username, user.Role);
                return new AuthResponse { Token = token, Username = user.Username, Role = user.Role };
            }
            catch (UnauthorizedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user: {Username}", dto.Username);
                throw new ApiExceptionHelper("An error occurred during login. Please try again later.", 500);
            }
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterUserRequest dto)
        {
            try
            {
                var existingUserByUsername = await _authRepository.GetUserByUsernameAsync(dto.Username);
                if (existingUserByUsername != null)
                    throw new ValidationException($"Username '{dto.Username}' is already taken.");

                var existingUserByEmail = await _authRepository.GetUserByEmailAsync(dto.Email);
                if (existingUserByEmail != null)
                    throw new ValidationException($"Email '{dto.Email}' is already registered.");

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };

                await _authRepository.AddUserAsync(user);
                await _authRepository.SaveChangesAsync();

                return new RegisterResponse { Username = user.Username };
            }
            catch (ValidationException valEx)
            {
                _logger.LogWarning(valEx, "Validation failed for registration: {Username}", dto.Username);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for user: {Username}", dto.Username);
                throw new ApiExceptionHelper("An error occurred during registration. Please try again later.", 500);
            }
        }
    }
}
