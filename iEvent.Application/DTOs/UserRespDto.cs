namespace iEvent.Application.DTOs
{
    public class UserRespDto
    {
        public string Id { get; set; }
        public Guid? CustomerId { get; set; } 
        public string Email { get; set; }
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; }

        public UserRespDto(string id, Guid? customerId, string email, string name, string? phoneNumber, List<string> roles)
        {
            Id = id;
            CustomerId = customerId; 
            Email = email;
            Name = name;
            PhoneNumber = phoneNumber;
            Roles = roles;
        }
    }
}
