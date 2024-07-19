using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Product;

public class ProductPutModel
{
    [Required]
    [Range(0, long.MaxValue)]
    public long? Id { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.RegularString)]
    public string? Name { get; set; }

    [Required]
    [Range(0, 20_000)]
    public decimal? Price { get; set; }

    [Required]
    [Range(0, 20_000)]
    public decimal? Weight { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.RegularString)]
    public string? MeasurementUnit { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.RegularString)]
    public string? Description { get; set; }

    [Required]
    [Range(0, long.MaxValue)]
    public long? CategoryId { get; set; }
}
