using Newtonsoft.Json;

namespace GeoCoding.Helpers
{
    public static class ObjectToStringJson
    {
        public static string GetStringOfObject(object obj)
        {
            string result = JsonConvert.SerializeObject(obj, Formatting.None);
            return result;
        }

        public static T GetObjectOfstring<T>(string str)
        {
            T result = JsonConvert.DeserializeObject<T>(str);
            return result;
        }
    }
}