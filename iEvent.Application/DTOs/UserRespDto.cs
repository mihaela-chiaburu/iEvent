namespace iEvent.Application.DTOs
{
    public record UserRespDto(string Id, string Name, string Email, string? PhoneNumber, IList<string> Roles);
}
