namespace Domain.Models.User;

public class UserModel : BaseModel
{
    public string FirstName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public long UserRoleId { get; set; }

    public string UserRole { get; set; } = string.Empty;
}
