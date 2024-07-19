using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Shop;

public class ShopPostModel
{
    [RegularExpression(RegularExpressions.Street)]
    public string? Street { get; set; }

    [RegularExpression(RegularExpressions.Building)]
    public string? Building { get; set; }

    [RegularExpression(RegularExpressions.Hour)]
    public string? OpeningTime { get; set; }

    [RegularExpression(RegularExpressions.Hour)]
    public string? ClosingTime { get; set; }
}
