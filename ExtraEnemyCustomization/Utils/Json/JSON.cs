﻿using EEC.Utils.Integrations;
using EEC.Utils.Json.Converters;
using ExtraEnemyCustomization.Utils.Integrations;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EEC.Utils.Json
{
    public static class JSON
    {
        private static readonly JsonSerializerOptions _setting = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            IncludeFields = false,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            IgnoreReadOnlyProperties = true
        };

        static JSON()
        {
            _setting.Converters.Add(new ColorConverter());
            _setting.Converters.Add(new LocalizedTextConverter());
            _setting.Converters.Add(new JsonStringEnumConverter());
            _setting.Converters.Add(new Vector2Converter());
            _setting.Converters.Add(new Vector3Converter());

            if (MTFOPartialDataUtil.IsLoaded && MTFOPartialDataUtil.Initialized)
            {
                _setting.Converters.Add(MTFOPartialDataUtil.PersistentIDConverter);
                Logger.Log("PartialData Support Found!");
            }
            if (InjectLibUtil.IsLoaded)
            {
                _setting.Converters.Add(InjectLibUtil.InjectLibConnector);
                Logger.Log("InjectLib found!");
            }
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _setting);
        }

        public static object Deserialize(Type type, string json)
        {
            return JsonSerializer.Deserialize(json, type, _setting);
        }

        public static string Serialize(object value, Type type)
        {
            return JsonSerializer.Serialize(value, type, _setting);
        }
    }
}