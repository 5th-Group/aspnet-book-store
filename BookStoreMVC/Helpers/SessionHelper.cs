using System.Text;
using Newtonsoft.Json;

namespace BookStoreMVC.Helpers
{
    public static class SessionHelper
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        
        public static void SetObjectAsBase64(this ISession session, string key, object value)
        {
            session.SetString(key, Convert.ToBase64String(Encoding.UTF8.GetBytes(value.ToString()!)));
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return string.IsNullOrEmpty(value) ? default : JsonConvert.DeserializeObject<T>(value)!;
        }
        
        public static T? GetObjectFromBase64<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return string.IsNullOrEmpty(value)
                ? default
                : JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(Convert.FromBase64String(value)));
        }
    }
}