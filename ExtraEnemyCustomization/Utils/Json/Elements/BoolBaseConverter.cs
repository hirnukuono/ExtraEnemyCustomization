using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EEC.Utils.Json.Elements
{
    public sealed class BoolBaseConverter : JsonConverter<BoolBase>
    {
        public override bool HandleNull => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ValueBase);
        }

        public override BoolBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var strValue = reader.GetString().Trim();
                    if (strValue.EqualsAnyIgnoreCase("Unchanged", "Ignore", "Keep", "Original", "KeepOriginal"))
                    {
                        return BoolBase.Unchanged;
                    }
                    else if (bool.TryParse(strValue, out var parsedValue))
                    {
                        return new BoolBase(parsedValue);
                    }
                    throw new JsonException($"Cannot parse BoolBase string: {strValue}! Are you sure it's in right format?");

                case JsonTokenType.True:
                    return BoolBase.True;

                case JsonTokenType.False:
                    return BoolBase.False;

                default:
                    throw new JsonException($"BoolBaseJson type: {reader.TokenType} is not implemented!");
            }
        }

        public override void Write(Utf8JsonWriter writer, BoolBase value, JsonSerializerOptions options)
        {
            switch (value.Mode)
            {
                case BoolMode.True:
                    writer.WriteBooleanValue(true);
                    break;

                case BoolMode.False:
                    writer.WriteBooleanValue(false);
                    break;

                case BoolMode.Unchanged:
                    writer.WriteStringValue("Unchanged");
                    break;
            }
        }
    }
}
