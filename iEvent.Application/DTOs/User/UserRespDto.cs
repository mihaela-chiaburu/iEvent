namespace iEvent.Application.DTOs.User
{
    public record UserRespDto(string Id, Guid? CustomerId, Guid? AdminId, string Email, string Name, string? PhoneNumber, List<string> Roles);
}
