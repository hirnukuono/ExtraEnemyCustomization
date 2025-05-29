using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EEC.Utils.Json.Elements
{
    public sealed class ExitConditionTargetConverter : JsonConverter<ExitConditionTarget>
    {
        private static readonly char[] _separators = new char[] { ',', '|' };

        public override ExitConditionTarget Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.False:
                    return ExitConditionTarget.None;

                case JsonTokenType.True:
                    return ExitConditionTarget.All;

                case JsonTokenType.String:
                    var strValue = reader.GetString();
                    var splitValues = strValue.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
                    if (splitValues.Length <= 0)
                    {
                        throw new JsonException($"There are no entries in {strValue}! Are you sure it's in right format?");
                    }

                    var target = ExitConditionType.None;
                    foreach (var str in splitValues)
                    {
                        var input = str.ToLowerInvariant().Trim();
                        switch (input)
                        {
                            case "allowedmode":
                            case "mode":
                                target |= ExitConditionType.Mode;
                                continue;

                            case "keepondead":
                            case "dead":
                                target |= ExitConditionType.Dead;
                                continue;

                            case "allowwhileattack":
                            case "attack":
                                target |= ExitConditionType.Attack;
                                continue;

                            case "state":
                                target |= ExitConditionType.State;
                                continue;

                            case "distance":
                                target |= ExitConditionType.Distance;
                                continue;
                        }
                    }
                    return new ExitConditionTarget(target);

                //Why?????
                case JsonTokenType.Number:
                    Logger.Warning("Found flag number value in ExitConditionTarget! : consider changing it to string.");
                    return new ExitConditionTarget((ExitConditionType)reader.GetInt32());

                default:
                    throw new JsonException($"Token type: {reader.TokenType} in ExitConditionTarget is not supported!");
            }
        }

        public override void Write(Utf8JsonWriter writer, ExitConditionTarget value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((int)value.Mode);
        }
    }
}