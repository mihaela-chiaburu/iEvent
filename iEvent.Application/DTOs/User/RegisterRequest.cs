namespace iEvent.Application.DTOs.User
{
    public record RegisterRequest(string Email, string Password, string UserName, string PhoneNumber);
}
