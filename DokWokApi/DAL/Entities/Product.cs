using System.ComponentModel.DataAnnotations.Schema;

namespace DokWokApi.DAL.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(8, 2)")]
    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;

    public long CategoryId { get; set; }

    public ProductCategory? Category { get; set; }
}
