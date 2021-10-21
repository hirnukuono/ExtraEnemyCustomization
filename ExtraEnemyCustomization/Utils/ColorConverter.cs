using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EECustom.Utils
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override bool HandleNull => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }

        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var color = new Color();

            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            return color;

                        if (reader.TokenType != JsonTokenType.PropertyName)
                            throw new JsonException("Expected PropertyName token");

                        var propName = reader.GetString();
                        reader.Read();

                        switch (propName.ToLower())
                        {
                            case "r":
                                color.r = reader.GetSingle();
                                break;

                            case "g":
                                color.g = reader.GetSingle();
                                break;

                            case "b":
                                color.b = reader.GetSingle();
                                break;

                            case "a":
                                color.a = reader.GetSingle();
                                break;
                        }
                    }
                    throw new JsonException("Expected EndObject token");

                case JsonTokenType.String:
                    var strValue = reader.GetString().Trim();
                    if (ColorUtility.TryParseHtmlString(strValue, out color))
                    {
                        return color;
                    }

                    if (TryParseColor(strValue, out color))
                    {
                        return color;
                    }
                    throw new JsonException($"Color format is not right: {strValue}");

                default:
                    throw new JsonException($"ColorJson type: {reader.TokenType} is not implemented!");
            }
        }

        private bool TryParseColor(string input, out Color color)
        {
            input = input.Trim().Trim('(',')');

            if (!input.Contains(",", StringComparison.Ordinal))
            {
                color = Color.clear;
                return false;
            }

            var split = input.Split(",");
            if (split.Length != 3 && split.Length != 4)
            {
                color = Color.clear;
                return false;
            }

            if (!float.TryParse(split[0].Trim(), out var r))
            {
                color = Color.clear;
                return false;
            }

            if (!float.TryParse(split[1].Trim(), out var g))
            {
                color = Color.clear;
                return false;
            }

            if (!float.TryParse(split[2].Trim(), out var b))
            {
                color = Color.clear;
                return false;
            }

            var a = 1.0f;
            if (split.Length == 4 && !float.TryParse(split[3].Trim(), out a))
            {
                color = Color.clear;
                return false;
            }

            color = new Color(r, g, b, a);
            return true;
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}