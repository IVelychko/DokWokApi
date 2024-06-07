using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.ProductCategory;

public class ProductCategoryModel : BaseModel
{
    public string Name { get; set; } = string.Empty;
}
