using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.ProductCategory;

public class ProductCategoryPutModel
{
    [Required]
    public long? Id { get; set; }

    [Required]
    public string? Name { get; set; }
}
