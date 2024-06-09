﻿using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Order;

public class OrderLinePostModel
{
    [Required]
    [Range(0, long.MaxValue)]
    public long? OrderId { get; set; }

    [Required]
    [Range(0, long.MaxValue)]
    public long? ProductId { get; set; }

    [Required]
    [Range(0, 200)]
    public int? Quantity { get; set; }
}
