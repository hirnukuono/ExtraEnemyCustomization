using EECustom.Extensions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EECustom.Utils.JsonElements
{
    public class ValueBaseConverter : JsonConverter<ValueBase>
    {
        public override bool HandleNull => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ValueBase);
        }

        public override ValueBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    return new ValueBase(reader.GetSingle(), ValueMode.Abs);

                case JsonTokenType.StartObject:

                    var valueBase = new ValueBase();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            return valueBase;

                        if (reader.TokenType != JsonTokenType.PropertyName)
                            throw new JsonException("Expected PropertyName token");

                        var propName = reader.GetString();
                        reader.Read();

                        switch (propName.ToLower())
                        {
                            case "value":
                                valueBase.Value = reader.GetSingle();
                                break;

                            case "mode":
                                if (Enum.TryParse<ValueMode>(reader.GetString(), out var valueMode))
                                {
                                    valueBase.Mode = valueMode;
                                }
                                break;

                            case "fromdefault":
                                valueBase.FromDefault = reader.GetBoolean();
                                break;
                        }
                    }
                    throw new JsonException("Expected EndObject token");

                case JsonTokenType.String:
                    var strValue = reader.GetString().Trim();
                    var fromDefaultFlag = false;

                    if (strValue.EndsWith("of default", StringComparison.OrdinalIgnoreCase))
                    {
                        fromDefaultFlag = true;
                        strValue = strValue[0..^10].TrimEnd();
                    }

                    if (strValue.EndsWith("%"))
                    {
                        if (float.TryParse(strValue[0..^1].TrimEnd(), out var parsedPercent))
                        {
                            return new ValueBase(parsedPercent / 100.0f, ValueMode.Rel, fromDefaultFlag);
                        }
                    }
                    else if (strValue.EqualsAnyIgnoreCase("Unchanged", "Ignore", "Keep", "Original", "KeepOriginal"))
                    {
                        return new ValueBase(1.0f, ValueMode.Rel, false);
                    }
                    else if (float.TryParse(strValue, out var parsedValue))
                    {
                        return new ValueBase(parsedValue, ValueMode.Abs);
                    }
                    throw new JsonException($"Cannot parse ValueBase string: {strValue}! Are you sure it's in right format?");

                default:
                    throw new JsonException($"ValueBaseJson type: {reader.TokenType} is not implemented!");
            }
        }

        public override void Write(Utf8JsonWriter writer, ValueBase value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}