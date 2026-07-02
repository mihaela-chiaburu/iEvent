namespace iEvent.Application.DTOs.User
{
    public record IdentityResultDto(bool Succeeded, IEnumerable<string>? Errors = null)
    {
        public string? CreatedUserId { get; init; }
    }
}