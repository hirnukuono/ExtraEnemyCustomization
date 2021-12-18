using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EECustom.Utils.JsonElements
{
    public class VoiceEventConverter : JsonConverter<VoiceEvent>
    {
        public override VoiceEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var strValue = reader.GetString();
                    return default;

                //Why?????
                case JsonTokenType.Number:
                    Logger.Warning("Found flag number value in AgentModeTarget! : consider changing it to string.");
                    return default;

                default:
                    throw new JsonException($"Token type: {reader.TokenType} in AgentModeTarget is not supported!");
            }
        }

        public override void Write(Utf8JsonWriter writer, VoiceEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
