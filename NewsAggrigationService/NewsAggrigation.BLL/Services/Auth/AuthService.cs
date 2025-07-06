using Microsoft.Extensions.Logging;
using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using NewsAggrigation.DAL.Models;
using NewsAggrigation.DAL.Repositories.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    return null;

                var token = _tokenService.CreateToken(user.UserId, user.Username, user.Role);
                return new AuthResponse { Token = token, Username = user.Username, Role = user.Role };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user: {Username}", dto.Username);
                throw new ApplicationException("An error occurred during login. Please try again later.");
            }
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterUserRequest dto)
        {
            try
            {
                var existingUserByUsername = await _authRepository.GetUserByUsernameAsync(dto.Username);
                if (existingUserByUsername != null)
                    throw new ApplicationException($"Username '{dto.Username}' is already taken.");

                var existingUserByEmail = await _authRepository.GetUserByEmailAsync(dto.Email);
                if (existingUserByEmail != null)
                    throw new ApplicationException($"Email '{dto.Email}' is already registered.");

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
            catch (ApplicationException appEx)
            {
                _logger.LogWarning(appEx, "Validation failed for registration: {Username}", dto.Username);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for user: {Username}", dto.Username);
                throw new ApplicationException("An error occurred during registration. Please try again later.");
            }
        }
    }
}
