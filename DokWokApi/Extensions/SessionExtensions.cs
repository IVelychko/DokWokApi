using System.Text.Json;

namespace DokWokApi.Extensions;

public static class SessionExtensions
{
    public static T? GetJson<T>(this ISession session, string key)
    {
        var sessionData = session.GetString(key);
        return sessionData is null ? default : JsonSerializer.Deserialize<T>(sessionData);
    }

    public static void SetJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }
}
