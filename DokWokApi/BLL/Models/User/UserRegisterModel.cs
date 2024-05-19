﻿using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class UserRegisterModel
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}