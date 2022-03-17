using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EEC.Utils.JsonElements
{
    internal sealed class EventWrapperConverter : JsonConverter<EventWrapper>
    {
        public override bool HandleNull => false;

        public override EventWrapper Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                return new EventWrapper(doc.RootElement.ToString());
            }
            throw new JsonException($"{reader.TokenType} is not supported for EventWrapperValue!");
        }

        public override void Write(Utf8JsonWriter writer, EventWrapper value, JsonSerializerOptions options)
        {
            return;
        }
    }
}