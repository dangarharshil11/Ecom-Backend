namespace IdentityService._2.BusinessLogic.DTO
{
    public class ChangePasswordRequestDto
    {
        public required string Email { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
