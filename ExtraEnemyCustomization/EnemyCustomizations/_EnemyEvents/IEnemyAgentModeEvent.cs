using Agents;
using Enemies;

namespace EECustom.EnemyCustomizations
{
    public interface IEnemyAgentModeEvent : IEnemyEvent
    {
        void OnAgentModeChanged(EnemyAgent agent, AgentMode newMode);
    }
}