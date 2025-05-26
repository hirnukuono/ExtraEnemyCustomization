using System.Text.Json.Serialization;

namespace EEC.Utils.Json.Elements
{
    [JsonConverter(typeof(ExitConditionTargetConverter))]
    public struct ExitConditionTarget
    {
        public static readonly ExitConditionTarget All = new(ExitConditionType.Mode | ExitConditionType.Dead | ExitConditionType.Attack | ExitConditionType.State | ExitConditionType.Distance);
        public static readonly ExitConditionTarget None = new(ExitConditionType.None);

        public ExitConditionType Mode;

        public ExitConditionTarget(ExitConditionType modes)
        {
            Mode = modes;
        }

        public bool HasFlag(ExitConditionType type) => Mode.HasFlag(type);
    }

    [Flags]
    public enum ExitConditionType
    {
        None = 0,
        Mode = 1,
        Dead = 1 << 1,
        Attack = 1 << 2,
        State = 1 << 3,
        Distance = 1 << 4
    }
}