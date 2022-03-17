using Agents;
using Enemies;

namespace EECustom.Customizations
{
    public interface IEnemyAgentModeEvent : IEnemyEvent
    {
        void OnAgentModeChanged(EnemyAgent agent, AgentMode newMode);
    }
}