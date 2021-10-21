using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EECustom.Utils
{
    //MINOR: Implement this
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var vector = new Vector2();

            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            return vector;

                        if (reader.TokenType != JsonTokenType.PropertyName)
                            throw new JsonException("Expected PropertyName token");

                        var propName = reader.GetString();
                        reader.Read();

                        switch (propName.ToLower())
                        {
                            case "x":
                                vector.x = reader.GetSingle();
                                break;

                            case "y":
                                vector.y = reader.GetSingle();
                                break;
                        }
                    }
                    throw new JsonException("Expected EndObject token");

                case JsonTokenType.String:
                    var strValue = reader.GetString().Trim();
                    if (TryParseVector2(strValue, out vector))
                    {
                        return vector;
                    }
                    throw new JsonException($"Vector2 format is not right: {strValue}");

                default:
                    throw new JsonException($"Vector2Json type: {reader.TokenType} is not implemented!");
            }
        }

        private bool TryParseVector2(string input, out Vector2 vector)
        {
            if (!input.Contains(",", StringComparison.Ordinal))
            {
                vector = Vector2.zero;
                return false;
            }

            var split = input.Split(",");
            if (split.Length != 2)
            {
                vector = Vector2.zero;
                return false;
            }

            if (!float.TryParse(split[0].Trim(), out var x))
            {
                vector = Vector2.zero;
                return false;
            }

            if (!float.TryParse(split[1].Trim(), out var y))
            {
                vector = Vector2.zero;
                return false;
            }

            vector = new Vector2(x, y);
            return true;
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
