using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Product;

public class ProductPostModel
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public decimal? Price { get; set; }

    [Required]
    public decimal? Weight { get; set; }

    [Required]
    public string? MeasurementUnit { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public long? CategoryId { get; set; }
}
