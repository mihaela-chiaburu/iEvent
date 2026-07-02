namespace iEvent.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string userId, string email, IList<string> roles);
    }
}
