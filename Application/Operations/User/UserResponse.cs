namespace Application.Operations.User;

public class UserResponse : BaseResponse
{
    public required string FirstName { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    public required long UserRoleId { get; set; }

    public required string UserRole { get; set; }
}
