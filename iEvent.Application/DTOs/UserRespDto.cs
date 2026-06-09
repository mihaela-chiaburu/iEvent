namespace iEvent.Application.DTOs
{
    public record UserRespDto(string Id, string Email, IList<string> Roles);
}
