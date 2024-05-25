﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DokWokApi.BLL.Models.Product;

public class ProductModel : BaseModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(8, 2)")]
    public decimal Price { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public long CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;
}
