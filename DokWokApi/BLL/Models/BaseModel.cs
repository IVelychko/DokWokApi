using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models;

public class BaseModel
{
    [Required]
    public long Id { get; set; }
}
