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
            if (!RegexUtil.TryParseVectorString(input, out var array))
            {
                color = Color.white;
                return false;
            }

            if (array.Length < 3)
            {
                color = Color.white;
                return false;
            }

            float alpha = 1.0f;
            if (array.Length > 3)
            {
                alpha = array[3];
            }

            color = new Color(array[0], array[1], array[2], alpha);
            return true;
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}