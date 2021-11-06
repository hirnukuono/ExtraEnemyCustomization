using EECustom.Utils.Integrations;
using EECustom.Utils.JsonConverters;
using EECustom.Utils.JsonElements;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EECustom.Utils
{
    public static class JSON
    {
        private readonly static JsonSerializerOptions _setting = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            IncludeFields = false,
            PropertyNameCaseInsensitive = true
        };

        static JSON()
        {
            _setting.Converters.Add(new ValueBaseConverter());
            _setting.Converters.Add(new AgentModeTargetConverter());
            _setting.Converters.Add(new ColorConverter());
            _setting.Converters.Add(new JsonStringEnumConverter());
            _setting.Converters.Add(new Vector2Converter());
            _setting.Converters.Add(new Vector3Converter());

            if (MTFOPartialDataUtil.IsLoaded && MTFOPartialDataUtil.Initialized)
            {
                _setting.Converters.Add(MTFOPartialDataUtil.PersistentIDConverter);
                Logger.Log("PartialData Support Found!");
            }
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _setting);
        }
    }
}