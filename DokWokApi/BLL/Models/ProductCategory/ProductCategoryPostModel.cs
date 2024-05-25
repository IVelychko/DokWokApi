using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.ProductCategory;

public class ProductCategoryPostModel
{
    [Required]
    public string? Name { get; set; }
}
