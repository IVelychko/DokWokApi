namespace DokWokApi.BLL.Models.Shop;

public class ShopModel : BaseModel
{
    public string Street { get; set; } = string.Empty;

    public string Building { get; set; } = string.Empty;

    public string OpeningTime { get; set; } = string.Empty;

    public string ClosingTime { get; set; } = string.Empty;
}
