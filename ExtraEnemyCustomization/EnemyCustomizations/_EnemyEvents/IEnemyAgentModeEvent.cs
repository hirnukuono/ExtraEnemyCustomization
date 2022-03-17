using Agents;
using Enemies;

namespace EEC.EnemyCustomizations
{
    public interface IEnemyAgentModeEvent : IEnemyEvent
    {
        void OnAgentModeChanged(EnemyAgent agent, AgentMode newMode);
    }
}