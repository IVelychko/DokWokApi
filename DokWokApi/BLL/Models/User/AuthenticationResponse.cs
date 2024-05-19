namespace DokWokApi.BLL.Models.User;

public class AuthenticationResponse
{
    public string FirstName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}
