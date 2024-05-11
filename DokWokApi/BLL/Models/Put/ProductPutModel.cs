using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Put;

public class ProductPutModel
{
    [Required]
    public long? Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    [Column(TypeName = "decimal(8, 2)")]
    public decimal? Price { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public long? CategoryId { get; set; }
}
