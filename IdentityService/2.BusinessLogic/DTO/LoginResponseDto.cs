namespace IdentityService._2.BusinessLogic.DTO
{
    public class LoginResponseDto
    {
        public required string UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public required string Token { get; set; }
        public required List<string> Roles { get; set; }
    }
}
