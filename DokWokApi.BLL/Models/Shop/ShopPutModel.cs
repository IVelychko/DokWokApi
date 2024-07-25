using System.ComponentModel.DataAnnotations;
using DokWokApi.BLL.Infrastructure;

namespace DokWokApi.BLL.Models.Shop;

public class ShopPutModel
{
    [Range(0, long.MaxValue)]
    public long? Id { get; set; }

    [RegularExpression(RegularExpressions.Street)]
    public string? Street { get; set; }

    [RegularExpression(RegularExpressions.Building)]
    public string? Building { get; set; }

    [RegularExpression(RegularExpressions.Hour)]
    public string? OpeningTime { get; set; }

    [RegularExpression(RegularExpressions.Hour)]
    public string? ClosingTime { get; set; }
}
