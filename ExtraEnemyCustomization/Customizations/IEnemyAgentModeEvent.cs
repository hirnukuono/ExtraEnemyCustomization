using Agents;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations
{
    public interface IEnemyAgentModeEvent : IEnemyEvent
    {
        void OnAgentModeChanged(EnemyAgent agent, AgentMode newMode);
    }
}
