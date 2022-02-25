using Agents;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Networking.Replicators
{
    public sealed class EnemyAgentModeReplicator : StateReplicator<EnemyState>
    {
        public override bool ClearOnLevelCleanup => true;
    }

    public struct EnemyState
    {
        public AgentMode mode;
    }
}
