using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.ProductCategory;

public class ProductCategoryPutModel
{
    [Required]
    [Range(0, long.MaxValue)]
    public long? Id { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.RegularString)]
    public string? Name { get; set; }
}
