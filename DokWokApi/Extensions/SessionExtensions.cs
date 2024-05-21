using System.Text.Json;

namespace DokWokApi.Extensions;

public static class SessionExtensions
{
    public static async Task<T?> GetJsonAsync<T>(this ISession session, string key)
    {
        if (!session.IsAvailable)
        {
            await session.LoadAsync();
        }

        var sessionData = session.GetString(key);
        return sessionData is null ? default : JsonSerializer.Deserialize<T>(sessionData);
    }

    public static async Task SetJsonAsync(this ISession session, string key, object value)
    {
        if (!session.IsAvailable)
        {
            await session.LoadAsync();
        }

        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static async Task<string?> GetStringAsync(this ISession session, string key)
    {
        if (!session.IsAvailable)
        {
            await session.LoadAsync();
        }

        var sessionData = session.GetString(key);
        return sessionData;
    }

    public static async Task SetStringAsync(this ISession session, string key, string value)
    {
        if (!session.IsAvailable)
        {
            await session.LoadAsync();
        }

        session.SetString(key, value);
    }

    public static async Task RemoveAsync(this ISession session, string key)
    {
        if (!session.IsAvailable)
        {
            await session.LoadAsync();
        }

        session.Remove(key);
    }
}
