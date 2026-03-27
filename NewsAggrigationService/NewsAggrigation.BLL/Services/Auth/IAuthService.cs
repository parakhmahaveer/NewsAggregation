using NewsAggrigation.API.ServiceDTOs.RequestDTOs;
using NewsAggrigation.API.ServiceDTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.BLL.Services.Auth
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterUserRequest dto);
        Task<AuthResponse?> LoginAsync(LoginRequest dto);
    }
}
