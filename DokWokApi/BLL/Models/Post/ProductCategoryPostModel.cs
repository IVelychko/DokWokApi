using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Post;

public class ProductCategoryPostModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
