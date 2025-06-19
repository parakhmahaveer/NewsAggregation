using NewsAggrigationClient.Models.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigationClient.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync();
        Task<TokenResponseDto?> LoginAsync();
    }
}
