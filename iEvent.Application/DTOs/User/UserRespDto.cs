namespace iEvent.Application.DTOs.User
{
    public record UserRespDto(string Id, Guid? CustomerId, string Email, string Name, string? PhoneNumber, List<string> Roles);
}
