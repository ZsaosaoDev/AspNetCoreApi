using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.DTOs;

namespace Asp.NETCoreApi.IRepositories {
    public interface IAccountRepository {
        Task<LogDto> SignUpAsync (SignUpDto model);
        Task<LogDto> SignInAsync (SignInDto model);
        Task<LogDto> RefreshTokenAsync (string refreshToken);
        Task LogoutAsync (string userId);
        Task<string> GetUserRoleAsync (string email);
    }
}
