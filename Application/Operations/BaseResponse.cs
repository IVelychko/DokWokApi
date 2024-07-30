namespace Application.Operations;

public class BaseResponse<TKey>
{
    public required TKey Id { get; set; }
}
