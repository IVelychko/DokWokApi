using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Put;

public class ProductCategoryPutModel : BaseModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
