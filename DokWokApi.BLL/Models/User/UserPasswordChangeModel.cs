﻿using System.ComponentModel.DataAnnotations;
using DokWokApi.BLL.Infrastructure;

namespace DokWokApi.BLL.Models.User;

public class UserPasswordChangeModel
{
    [Required]
    [RegularExpression(RegularExpressions.Guid)]
    public string? UserId { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.Password)]
    public string? OldPassword { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.Password)]
    public string? NewPassword { get; set; }
}
