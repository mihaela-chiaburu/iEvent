namespace iEvent.Application.DTOs.User
{
    public class AuthLoginRespDto
    {
        public string Token { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}