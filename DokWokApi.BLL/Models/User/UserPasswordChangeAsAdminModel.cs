﻿using System.ComponentModel.DataAnnotations;
using DokWokApi.BLL.Infrastructure;

namespace DokWokApi.BLL.Models.User;

public class UserPasswordChangeAsAdminModel
{
    [Required]
    [RegularExpression(RegularExpressions.Guid)]
    public string? UserId { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.Password)]
    public string? NewPassword { get; set; }
}
