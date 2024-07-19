using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.ProductCategory;

public class ProductCategoryPostModel
{
    [Required]
    [RegularExpression(RegularExpressions.RegularString)]
    public string? Name { get; set; }
}
