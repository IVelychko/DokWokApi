﻿namespace DokWokApi.BLL.Models.User;

public class AuthorizedUserModel
{
    public string Id { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string Token { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}
