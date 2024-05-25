using DokWokApi.Exceptions;

namespace DokWokApi.BLL;

public static class ServiceHelper
{
    public static void CheckForNull<T>(T? model, string errorMessage)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model), errorMessage);
        }
    }

    public static void ThrowIfTrue(bool value, string errorMessage)
    {
        if (value)
        {
            throw new OrderException(nameof(value), errorMessage);
        }
    }
}
