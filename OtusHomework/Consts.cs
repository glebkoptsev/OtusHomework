using System.Text.Json;

namespace OtusHomework
{
    public static class Consts
    {
        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new(JsonSerializerDefaults.Web);
    }
}
