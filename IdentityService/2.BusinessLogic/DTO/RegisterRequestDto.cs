namespace IdentityService._2.BusinessLogic.DTO
{
    public class RegisterRequestDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
    }
}
