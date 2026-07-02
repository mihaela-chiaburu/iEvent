namespace iEvent.Application.DTOs.User
{
    public class UserFilterDto
    {
        public string? Search { get; set; }
        public string? Role { get; set; }
        public string? FilterByStatus { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
