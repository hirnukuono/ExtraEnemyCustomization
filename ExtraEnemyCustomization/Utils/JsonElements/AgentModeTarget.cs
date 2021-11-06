using Agents;
using Enemies;
using System;

namespace EECustom.Utils.JsonElements
{
    public struct AgentModeTarget
    {
        public readonly static AgentModeTarget None = new(AgentModeType.None);

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
