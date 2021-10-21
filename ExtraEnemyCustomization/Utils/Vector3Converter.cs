using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EECustom.Utils
{
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override bool HandleNull => false;

        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var vector = new Vector3();

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

                            case "z":
                                vector.z = reader.GetSingle();
                                break;
                        }
                    }
                    throw new JsonException("Expected EndObject token");

                case JsonTokenType.String:
                    var strValue = reader.GetString().Trim();
                    if (TryParseVector3(strValue, out vector))
                    {
                        return vector;
                    }
                    throw new JsonException($"Vector3 format is not right: {strValue}");

                default:
                    throw new JsonException($"Vector3Json type: {reader.TokenType} is not implemented!");
            }
        }

        private bool TryParseVector3(string input, out Vector3 vector)
        {
            input = input.Trim().Trim('(', ')');

            if (!input.Contains(",", StringComparison.Ordinal))
            {
                vector = Vector3.zero;
                return false;
            }

            var split = input.Split(",");
            if (split.Length != 3)
            {
                vector = Vector3.zero;
                return false;
            }

            if (!float.TryParse(split[0].Trim(), out var x))
            {
                vector = Vector3.zero;
                return false;
            }

            if (!float.TryParse(split[1].Trim(), out var y))
            {
                vector = Vector3.zero;
                return false;
            }

            if (!float.TryParse(split[2].Trim(), out var z))
            {
                vector = Vector3.zero;
                return false;
            }

            vector = new Vector3(x, y, z);
            return true;
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
