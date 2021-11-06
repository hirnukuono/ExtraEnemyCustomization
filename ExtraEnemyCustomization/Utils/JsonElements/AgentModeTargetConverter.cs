using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EECustom.Utils.JsonElements
{
    public class AgentModeTargetConverter : JsonConverter<AgentModeTarget>
    {
        public override AgentModeTarget Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var strValue = reader.GetString();
                    var splitValues = strValue.Split(',', '|', StringSplitOptions.RemoveEmptyEntries);
                    if (splitValues.Length <= 0)
                    {
                        throw new JsonException($"There are no entries in {strValue}! Are you sure it's in right format?");
                    }

                    var target = AgentModeType.None;
                    foreach (var str in splitValues)
                    {
                        var input = str.ToLower().Trim();
                        switch (input)
                        {
                            case "off":
                            case "dead":
                                target |= AgentModeType.Off;
                                break;

                            case "agressive":
                            case "combat":
                                target |= AgentModeType.Agressive;
                                break;

                            case "hibernate":
                            case "hibernation":
                            case "hibernating":
                            case "sleeping":
                                target |= AgentModeType.Hibernate;
                                break;

                            case "scout":
                            case "scoutpatrolling":
                                target |= AgentModeType.Scout;
                                break;

                            case "patrolling":
                                target |= AgentModeType.Patrolling;
                                break;
                        }
                    }
                    return new AgentModeTarget(target);

                //Why?????
                case JsonTokenType.Number:
                    Logger.Warning("Found flag number value in AgentModeTarget! : consider changing it to string.");
                    return new AgentModeTarget((AgentModeType)reader.GetInt32());

                default:
                    throw new JsonException($"Token type: {reader.TokenType} in AgentModeTarget is not supported!");
            }
        }

        public override void Write(Utf8JsonWriter writer, AgentModeTarget value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
