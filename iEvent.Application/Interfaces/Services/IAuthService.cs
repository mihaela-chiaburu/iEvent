using iEvent.Application.DTOs.User;

namespace iEvent.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);
        Task<AuthLoginRespDto> LoginAsync(LoginRequest request);
    }
}