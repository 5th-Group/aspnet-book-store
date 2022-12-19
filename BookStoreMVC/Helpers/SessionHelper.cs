using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json;

namespace BookStoreMVC.Helpers
{
    public static class SessionHelper
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return string.IsNullOrEmpty(value) ? default : JsonConvert.DeserializeObject<T>(value)!;
        }
    }
}