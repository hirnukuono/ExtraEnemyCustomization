using System.Text.Json;
using System.Text.Json.Serialization;

namespace EECustom.Utils
{
    public static class JSON
    {
        private readonly static JsonSerializerOptions _Setting = new JsonSerializerOptions()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            IncludeFields = false,
            PropertyNameCaseInsensitive = true
        };

        static JSON()
        {
            _Setting.Converters.Add(new ValueBaseConverter());
            _Setting.Converters.Add(new ColorConverter());
            _Setting.Converters.Add(new JsonStringEnumConverter());
            _Setting.Converters.Add(new Vector2Converter());
            _Setting.Converters.Add(new Vector3Converter());

            if (MTFOPartialDataUtil.IsLoaded && MTFOPartialDataUtil.Initialized)
            {
                _Setting.Converters.Add(MTFOPartialDataUtil.PersistentIDConverter);
                Logger.Log("PartialData Support Found!");
            }
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _Setting);
        }
    }
}