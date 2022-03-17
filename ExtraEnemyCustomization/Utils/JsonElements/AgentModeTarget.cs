using Agents;
using Enemies;
using System;
using System.Text.Json.Serialization;

namespace EEC.Utils.JsonElements
{
    [JsonConverter(typeof(AgentModeTargetConverter))]
    public struct AgentModeTarget
    {
        public static readonly AgentModeTarget All = new(AgentModeType.Off | AgentModeType.Hibernate | AgentModeType.Agressive | AgentModeType.Scout | AgentModeType.Patrolling);
        public static readonly AgentModeTarget Scout = new(AgentModeType.Scout);
        public static readonly AgentModeTarget Hibernate = new(AgentModeType.Hibernate);
        public static readonly AgentModeTarget Agressive = new(AgentModeType.Agressive);
        public static readonly AgentModeTarget None = new(AgentModeType.None);

        public AgentModeType Mode;

        public AgentModeTarget(AgentModeType modes)
        {
            Mode = modes;
        }

        public bool IsMatch(EnemyAgent agent)
        {
            if (agent == null)
                return false;

            return agent.AI.Mode switch
            {
                AgentMode.Off => Mode.HasFlag(AgentModeType.Off),
                AgentMode.Hibernate => Mode.HasFlag(AgentModeType.Hibernate),
                AgentMode.Agressive => Mode.HasFlag(AgentModeType.Agressive),
                AgentMode.Scout => Mode.HasFlag(AgentModeType.Scout),
                AgentMode.Patrolling => Mode.HasFlag(AgentModeType.Patrolling),
                _ => false,
            };
        }
    }

    [Flags]
    public enum AgentModeType
    {
        None = 0,
        Off = 1,
        Hibernate = 2,
        Agressive = 4,
        Scout = 8,
        Patrolling = 16
    }
}